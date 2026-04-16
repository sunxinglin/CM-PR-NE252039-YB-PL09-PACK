from s7svrsim import hints

def main(ctx :hints.S7Context):
    S7 = ctx.DBService  
    DB_INDEX= 200

    S7.WriteString(DB_INDEX, 3102, 24, "001PBCXX000002F9J0100001")  # UserCardId
    S7.WriteString(DB_INDEX, 3128, 24, "66554433")                  # WorkId
    S7.WriteShort(DB_INDEX, 3154, 1)                            # 权限等级

    # 触发信号
    S7.WriteBit(DB_INDEX, 3100, 0, True)
