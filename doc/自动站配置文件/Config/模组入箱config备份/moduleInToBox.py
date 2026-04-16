import time
from s7svrsim import hints

# ===== 仿真环境注入 =====
S7: hints.S7DB = S7
Logger: hints.Logger = Logger

DB_INDEX_CTRL = 200

def write_strings():
    S7.WriteBit(DB_INDEX_CTRL, 600, 1, True)
    S7.WriteBit(DB_INDEX_CTRL, 602, 1, True)

    S7.WriteString(DB_INDEX_CTRL, 628, 24, "11111111111111111")
    S7.WriteString(DB_INDEX_CTRL, 654, 24, "22222222222222222")

    base = 680
    for i in range(14):
        S7.WriteReal(DB_INDEX_CTRL, base + i * 4, 1.11111)

    Logger.LogInfo("DB200 写入完成")

# ⚠️ ScriptEngine / ScopeStorage 强制入口
def main(scope):
    write_strings()
