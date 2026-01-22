@echo off
setlocal enabledelayedexpansion
title Aura Checker 

:: Garante que o script rode na pasta onde o .bat esta localizado
cd /d "%~dp0"

cls
echo ============================================================
echo             LANCELOT CHECKER - INICIALIZADOR
echo ============================================================
echo.
echo [*] Diretorio: %cd%
echo [*] Verificando dependencias...

:: Tenta instalar as dependencias silenciosamente
python -m pip install -r requirements.txt "httpx[http2]" --quiet

if %errorlevel% neq 0 (
    echo.
    echo [!] Erro ao instalar dependencias automaticamente.
    echo [!] Tentando rodar o script mesmo assim...
    echo.
) else (
    echo [*] Dependencias OK!
)

echo [*] Iniciando o script...
echo.

:: Executa o script Python
python run.py

if %errorlevel% neq 0 (
    echo.
    echo [!] O script encerrou com um erro.
)

echo.
echo ============================================================
echo             EXECUCAO FINALIZADA
echo ============================================================
pause
