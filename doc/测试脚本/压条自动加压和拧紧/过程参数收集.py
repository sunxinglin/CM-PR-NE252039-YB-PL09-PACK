from s7svrsim import hints

def main(ctx :hints.S7Context):
    S7 = ctx.DBService  
    DB_INDEX= 200

    S7.WriteString(DB_INDEX, 602, 24, "001PBCXX000002F9J0100001")

    # 开始时间类型为DTL
    S7.WriteShort(DB_INDEX, 628, 2026)  # 年
    S7.WriteByte(DB_INDEX, 630, 3)      # 月
    S7.WriteByte(DB_INDEX, 631, 25)     # 日
    S7.WriteByte(DB_INDEX, 633, 17)     # 时
    S7.WriteByte(DB_INDEX, 634, 28)     # 分
    S7.WriteByte(DB_INDEX, 635, 39)     # 秒

    # 过程参数1~10
    GCCS_POSITION_OFFSET = 640
    for i in range(10):
        S7.WriteReal(DB_INDEX, GCCS_POSITION_OFFSET + i * 4, i)

    #拧紧结果1~30
    NJJG_POSITION_OFFSET = 680
    for i in range(30):
        S7.WriteShort(DB_INDEX, NJJG_POSITION_OFFSET, i % 2)    # 拧紧结果
        NJJG_POSITION_OFFSET += 2
        S7.WriteShort(DB_INDEX, NJJG_POSITION_OFFSET, i + 1)    # 螺丝序号
        NJJG_POSITION_OFFSET += 2
        S7.WriteShort(DB_INDEX, NJJG_POSITION_OFFSET, 10)       # 程序号
        NJJG_POSITION_OFFSET += 2
        S7.WriteReal(DB_INDEX, NJJG_POSITION_OFFSET, 10)        # 扭矩结果
        NJJG_POSITION_OFFSET += 4
        S7.WriteReal(DB_INDEX, NJJG_POSITION_OFFSET, 90)        # 角度结果
        NJJG_POSITION_OFFSET += 4


    # 触发信号
    S7.WriteBit(DB_INDEX, 600, 0, True)
