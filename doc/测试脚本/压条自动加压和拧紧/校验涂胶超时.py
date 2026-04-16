from s7svrsim import hints

def main(ctx :hints.S7Context):
    S7 = ctx.DBService  
    DB_INDEX= 200

    S7.WriteString(DB_INDEX, 1202, 24, "001PBCXX000002F9J0100001")

    # 触发信号
    S7.WriteBit(DB_INDEX, 1200, 0, True)
