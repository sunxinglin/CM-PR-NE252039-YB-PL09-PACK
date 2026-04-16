from s7svrsim import hints

def main(ctx :hints.S7Context):
    S7 = ctx.DBService
    DB_INDEX= 200

    # 开始时间，类型：DTL
    S7.WriteString(DB_INDEX, 622, 24, "001PBCXX000002F9J0100001")
    S7.WriteShort(DB_INDEX, 648, 2026)  # 年
    S7.WriteByte(DB_INDEX, 650, 3)      # 月
    S7.WriteByte(DB_INDEX, 651, 25)     # 日
    S7.WriteByte(DB_INDEX, 653, 17)     # 时
    S7.WriteByte(DB_INDEX, 654, 28)     # 分
    S7.WriteByte(DB_INDEX, 655, 39)     # 秒

    # 过程参数，类型：涂胶Array参数
    i = 0
    GCCS_START_OFFSET = 660
    while i <= 20:
        S7.WriteReal(DB_INDEX, GCCS_START_OFFSET + (i + 0) * 4, 10)         # A胶流量
        S7.WriteReal(DB_INDEX, GCCS_START_OFFSET + (i + 1) * 4, 15)         # B胶流量
        S7.WriteReal(DB_INDEX, GCCS_START_OFFSET + (i + 2) * 4, 1.5)        # AB胶比例
        S7.WriteReal(DB_INDEX, GCCS_START_OFFSET + (i + 3) * 4, 10 + 15)    # AB胶总胶重
        i += 1
        GCCS_START_OFFSET += 12

    # 触发信号
    S7.WriteBit(DB_INDEX, 620, 0, True)
