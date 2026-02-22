
# -*- coding: UTF-8 -*-
# define constants

import time


DB_INDEX= 40000

# 发送请求

# # 请求小车进站
# S7.WriteShort(DB_INDEX,202,1)
# S7.WriteString(DB_INDEX, 204, 38,'111222333')
# S7.WriteBit(DB_INDEX, 200,0, True)
# time.sleep(5)
# S7.WriteBit(DB_INDEX, 200,0, False)

# # 请求开始加压
# S7.WriteShort(DB_INDEX,302,1)
# S7.WriteString(DB_INDEX, 304, 38,'111222333')
# S7.WriteBit(DB_INDEX, 300,0, True)
# time.sleep(5)
# S7.WriteBit(DB_INDEX, 300,0, False)

# 请求入箱完成
S7.WriteString(DB_INDEX , 444 + 94 * 0, 38, "564646546465")#block条码
S7.WriteShort(DB_INDEX,484 + 94 * 0,1000) #保压时长
S7.WriteReal(DB_INDEX,488 + 94 * 0,10.5) #模组长度
S7.WriteReal(DB_INDEX,492 + 94 * 0,500.69) #最大压力
S7.WriteReal(DB_INDEX,496 + 94 * 0,400.36) #平均压力
S7.WriteReal(DB_INDEX,500 + 94 * 0,600.54) #夹紧力压力


S7.WriteShort(DB_INDEX,402,1)
S7.WriteString(DB_INDEX, 404, 38,'111222333')
S7.WriteBit(DB_INDEX, 400,0, True)

time.sleep(5)
S7.WriteBit(DB_INDEX, 400,0, False)
