from s7svrsim import hints

def main(ctx :hints.S7Context):
    S7 = ctx.DBService  
    DB_INDEX= 200

    S7.WriteString(DB_INDEX, 302, 24, "001PBCXX000002F9J0100001")   # PACK码
    S7.WriteString(DB_INDEX, 328, 24, "001MBCXT000002F9K0100001")   # 模组码

    # 触发信号
    S7.WriteBit(DB_INDEX, 100, 0, True)
