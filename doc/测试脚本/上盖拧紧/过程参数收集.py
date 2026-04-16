from s7svrsim import hints

def main(ctx :hints.S7Context):
    S7 = ctx.DBService  
    DB_INDEX= 200

    S7.WriteString(DB_INDEX, 602, 24, "001PBCXX000002F9J0100001")

    # 拧紧结果，类型：拧紧Array参数
    NJJG_POSITION_OFFSET = 628
    for i in range(150):
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
