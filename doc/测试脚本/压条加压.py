
# -*- coding: UTF-8 -*-
# define constants

import time


DB_INDEX= 200

# 发送请求

# # 请求小车进站
# S7.WriteBit(DB_INDEX, 114, 0, True)
# S7.WriteShort(DB_INDEX,116,1)
# S7.WriteString(DB_INDEX, 118, 30,'111110222')
# time.sleep(10)
# S7.WriteBit(DB_INDEX, 114, 0, False)

# # 请求开始加压
# S7.WriteBit(DB_INDEX, 180, 0, True)
# S7.WriteShort(DB_INDEX,182,1)
# S7.WriteString(DB_INDEX, 184, 38,'111110222')
# time.sleep(10)
# S7.WriteBit(DB_INDEX, 180, 0, False)

# #请求加压完成
S7.WriteShort(DB_INDEX,294,2024)
S7.WriteShort(DB_INDEX,296,9)
S7.WriteShort(DB_INDEX,298,22)
S7.WriteShort(DB_INDEX,300,10)
S7.WriteShort(DB_INDEX,302,19)
S7.WriteShort(DB_INDEX,304,59)
S7.WriteReal(DB_INDEX,306, 200.28)
S7.WriteReal(DB_INDEX,310, 220.28)
S7.WriteReal(DB_INDEX,314, 230.28)
S7.WriteReal(DB_INDEX,318, 210.28)
S7.WriteReal(DB_INDEX,322, 220.2234)
S7.WriteReal(DB_INDEX,326, 230.238)
S7.WriteReal(DB_INDEX,330, 280.28)
S7.WriteReal(DB_INDEX,334, 245.28)
S7.WriteReal(DB_INDEX,338, 234.28)
S7.WriteReal(DB_INDEX,342, 224.28)
S7.WriteReal(DB_INDEX,346, 254.28)
S7.WriteReal(DB_INDEX,350, 267.28)
S7.WriteReal(DB_INDEX,354, 289.28)
S7.WriteReal(DB_INDEX,358, 205.28)
S7.WriteReal(DB_INDEX,362, 2004.28)
S7.WriteReal(DB_INDEX,366, 2050.28)
S7.WriteReal(DB_INDEX,370, 2400.28)
S7.WriteReal(DB_INDEX,374, 2005.28)
S7.WriteReal(DB_INDEX,378, 2060.28)
S7.WriteReal(DB_INDEX,382, 2200.28)

S7.WriteBit(DB_INDEX, 250, 0, True)
S7.WriteShort(DB_INDEX,252,1)
S7.WriteString(DB_INDEX, 254, 38,'111110222')
time.sleep(10)
S7.WriteBit(DB_INDEX, 250, 0, False)
