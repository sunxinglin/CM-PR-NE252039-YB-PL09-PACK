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
    S7.WriteReal(DB_INDEX, 542, 11.11)
    S7.WriteReal(DB_INDEX, 546, 12.12)
    S7.WriteReal(DB_INDEX, 550, 13.13)
    S7.WriteReal(DB_INDEX, 554, 14.14)
    S7.WriteReal(DB_INDEX, 558, 15.15)
    S7.WriteReal(DB_INDEX, 562, 16.16)
    S7.WriteReal(DB_INDEX, 566, 17.17)
    S7.WriteReal(DB_INDEX, 570, 18.18)
    S7.WriteReal(DB_INDEX, 574, 19.19)
    S7.WriteReal(DB_INDEX, 578, 20.20)
    S7.WriteReal(DB_INDEX, 582, 21.21)

    # 触发信号
    S7.WriteBit(DB_INDEX, START_SIGNAL_OFFSET, 0, True)