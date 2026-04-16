from s7svrsim import hints

def main(ctx :hints.S7Context):
    S7 = ctx.DBService
    DB_INDEX= 200
    START_SIGNAL_OFFSET = 500

    # 首件数据收集
    S7.WriteReal(DB_INDEX, 502, 1.1)
    S7.WriteReal(DB_INDEX, 506, 2.2)
    S7.WriteReal(DB_INDEX, 510, 3.3)
    S7.WriteReal(DB_INDEX, 514, 4.4)
    S7.WriteReal(DB_INDEX, 518, 5.5)
    S7.WriteReal(DB_INDEX, 522, 6.6)
    S7.WriteReal(DB_INDEX, 526, 7.7)
    S7.WriteReal(DB_INDEX, 530, 8.8)
    S7.WriteReal(DB_INDEX, 534, 9.9)
    S7.WriteReal(DB_INDEX, 538, 10.10)

    # 触发信号
    S7.WriteBit(DB_INDEX, START_SIGNAL_OFFSET, 0, True)