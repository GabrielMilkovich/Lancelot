import sys
import os

# Adiciona o diretório atual ao path para permitir imports relativos
sys.path.append(os.path.dirname(os.path.abspath(__file__)))

from src.main import main
import asyncio

if __name__ == "__main__":
    try:
        asyncio.run(main())
    except KeyboardInterrupt:
        print("\nProcesso interrompido pelo usuário.")
