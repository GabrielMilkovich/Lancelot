import httpx
import asyncio
import time
import logging
from typing import Optional

# Chave de API do 2Captcha (Substitua pela sua chave real)
# Para fins de demonstração, usaremos uma chave placeholder.
# O usuário DEVE fornecer a chave real para que funcione.
API_KEY = "SEU_API_KEY_DO_2CAPTCHA_AQUI" 

# Configurações do Arkose Labs para o login da Microsoft
ARKOSE_PUBLIC_KEY = "B7D8911C-5CC8-A9A3-35B0-554ACEE604DA"
ARKOSE_SURL = "https://client-api.arkoselabs.com"
PAGE_URL = "https://login.live.com"

async def solve_arkose_captcha(client: httpx.AsyncClient) -> Optional[str]:
    """
    Resolve o CAPTCHA do Arkose Labs (FunCaptcha) usando a API do 2Captcha.
    Retorna o token de solução (token) ou None em caso de falha.
    """
    if API_KEY == "SEU_API_KEY_DO_2CAPTCHA_AQUI":
        # logging.error("API_KEY do 2Captcha não configurada. Bypass de CAPTCHA desativado.")
        return None

    logging.info("Tentando resolver CAPTCHA do Arkose Labs via 2Captcha...")

    # 1. Enviar a tarefa para o 2Captcha
    submit_url = "https://2captcha.com/in.php"
    submit_payload = {
        "key": API_KEY,
        "method": "funcaptcha",
        "publickey": ARKOSE_PUBLIC_KEY,
        "surl": ARKOSE_SURL,
        "pageurl": PAGE_URL,
        "json": 1
    }

    try:
        response = await client.post(submit_url, data=submit_payload, timeout=15.0)
        response.raise_for_status()
        result = response.json()
    except Exception as e:
        logging.error(f"Erro ao enviar tarefa para 2Captcha: {e}")
        return None

    if result.get("status") != 1:
        logging.error(f"2Captcha retornou erro ao submeter: {result.get('request')}")
        return None

    captcha_id = result.get("request")
    logging.info(f"Tarefa de CAPTCHA submetida. ID: {captcha_id}")

    # 2. Polling para obter o resultado
    get_url = "https://2captcha.com/res.php"
    get_params = {
        "key": API_KEY,
        "action": "get",
        "id": captcha_id,
        "json": 1
    }

    # Espera inicial de 5 segundos + polling
    await asyncio.sleep(5) 
    
    for _ in range(10): # Tenta por até 10 vezes (total de ~55 segundos)
        try:
            response = await client.get(get_url, params=get_params, timeout=15.0)
            response.raise_for_status()
            result = response.json()
        except Exception as e:
            logging.error(f"Erro ao fazer polling no 2Captcha: {e}")
            await asyncio.sleep(5)
            continue

        if result.get("status") == 1:
            token = result.get("request")
            logging.info("CAPTCHA resolvido com sucesso!")
            return token
        
        if result.get("request") not in ["CAPCHA_NOT_READY", "captcha_not_ready"]:
            logging.error(f"2Captcha retornou erro ao obter resultado: {result.get('request')}")
            return None

        await asyncio.sleep(5) # Espera 5 segundos antes de tentar novamente

    logging.error("Tempo limite excedido para resolver o CAPTCHA.")
    return None

# Função de exemplo para uso no main.py (não será usada diretamente aqui)
async def main_test():
    async with httpx.AsyncClient() as client:
        token = await solve_arkose_captcha(client)
        if token:
            print(f"Token de Solução: {token}")
        else:
            print("Falha ao obter token.")

if __name__ == "__main__":
    asyncio.run(main_test())
