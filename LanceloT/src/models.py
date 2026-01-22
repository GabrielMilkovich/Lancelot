from dataclasses import dataclass, field, asdict
from enum import Enum
from typing import List, Optional
import json

class LoginStatus(Enum):
    LOGADO = "Logado"
    ERRO_CREDENCIAIS = "Erro de Credenciais"
    SESSAO_BLOQUEADA = "Sessão Bloqueada"
    ERRO_REDE = "Erro de Rede"

class Plataforma(Enum):
    MICROSOFT = "Microsoft"

@dataclass
class Account:
    email: str
    password: str
    plataforma: Plataforma = Plataforma.MICROSOFT

@dataclass
class LoginResult:
    status: LoginStatus
    email: str
    password: str
    error_message: Optional[str] = None
    balance: str = "N/A"
    region: str = "N/A"
    games: List[str] = field(default_factory=list)
    timestamp: str = field(default_factory=lambda: "")

    def to_dict(self):
        return {
            "email": self.email,
            "status": self.status.value,
            "balance": self.balance,
            "region": self.region,
            "games": self.games,
            "error": self.error_message
        }

    def to_csv_line(self):
        games_joined = "|".join(self.games).replace(",", ";")
        return f"{self.email},{self.password},{self.status.value},{self.balance},{self.region},{games_joined}\n"
