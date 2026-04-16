from s7svrsim import hints

def main(ctx :hints.S7Context):
    S7 = ctx.DBService  
    DB_INDEX= 200

    S7.WriteString(DB_INDEX, 902, 24, "001PBCXX000002F9J0100001")  # UserCardId
    S7.WriteString(DB_INDEX, 928, 24, "66554433")                  # WorkId
    S7.WriteString(DB_INDEX, 954, 2, 1)                            # 权限等级

    # 触发信号
    S7.WriteBit(DB_INDEX, 900, 0, True)
