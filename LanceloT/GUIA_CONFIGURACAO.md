# Guia de Configuração Avançada do Lancelot

O Lancelot está agora equipado com as tecnologias mais avançadas para bypass de detecção. Para maximizar sua taxa de sucesso e evitar o status **LOCK**, siga estas instruções de configuração:

## 1. Configuração da API Key do 2Captcha

O Lancelot usa o 2Captcha para resolver automaticamente o desafio de segurança **Arkose Labs (FunCaptcha)** da Microsoft.

1.  **Obtenha sua Chave:** Crie uma conta no 2Captcha e obtenha sua chave de API.
2.  **Edite o Arquivo:** Abra o arquivo `src/captcha_solver.py`.
3.  **Substitua a Chave:** Localize a linha:
    ```python
    API_KEY = "SEU_API_KEY_DO_2CAPTCHA_AQUI"
    ```
    E substitua pelo seu token real:
    ```python
    API_KEY = "SUA_CHAVE_REAL_AQUI"
    ```

## 2. Configuração de Proxies Rotativos

O uso de proxies residenciais é **fundamental** para evitar que a Microsoft bloqueie seu IP por excesso de requisições.

1.  **Crie o Arquivo:** Crie um novo arquivo chamado `proxies.txt` na pasta `LanceloT/`.
2.  **Formato:** Adicione seus proxies, um por linha, no formato:
    - `IP:PORT`
    - `IP:PORT:USER:PASS`

    O Lancelot suporta automaticamente os protocolos `http://`, `https://` e `socks5://`. Se o protocolo não for especificado, ele assumirá `http://`.

    **Exemplo de `proxies.txt`:**
    ```
    192.168.1.1:8080
    user1:pass1@203.0.113.45:3128
    socks5://user2:pass2@198.51.100.10:1080
    ```

O Lancelot rotacionará um proxy diferente para cada conta, garantindo a máxima resiliência contra bloqueios de IP.

## 3. Uso do Lancelot

1.  Coloque suas contas no formato `email:senha` no arquivo `accounts.txt`.
2.  Execute o `run.py` (ou `INICIAR.bat` no Windows).
3.  Os resultados serão salvos em `aura.txt` (formato simples) e `results.json` (formato estruturado).
