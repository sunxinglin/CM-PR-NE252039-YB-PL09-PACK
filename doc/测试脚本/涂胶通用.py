# -*- coding: UTF-8 -*-
# define constants

import time

DB_INDEX= 200

# 请求小车进站
S7.WriteBit(DB_INDEX, 114, 0, True)
S7.WriteShort(DB_INDEX,116,1)
S7.WriteString(DB_INDEX, 118, 38,'111110222')
time.sleep(10)
S7.WriteBit(DB_INDEX, 114, 0, False)

# # 请求小车进站
# S7.WriteBit(DB_INDEX, 114, 0, True)
# S7.WriteShort(DB_INDEX,116,2)
# S7.WriteString(DB_INDEX, 118, 38,'111110223')
# time.sleep(10)
# S7.WriteBit(DB_INDEX, 114, 0, False)

# # # 请求开始涂胶
# S7.WriteBit(DB_INDEX, 180, 0, True)
# S7.WriteShort(DB_INDEX,182,2)
# S7.WriteString(DB_INDEX, 184, 38,'111110223')
# time.sleep(10)
# S7.WriteBit(DB_INDEX, 180, 0, False)

# # #请求加压完成
# S7.WriteShort(DB_INDEX, 294, 2024)
# S7.WriteShort(DB_INDEX, 296, 10)
# S7.WriteShort(DB_INDEX, 298, 15)
# S7.WriteShort(DB_INDEX, 300, 19)
# S7.WriteShort(DB_INDEX, 302, 50)
# S7.WriteShort(DB_INDEX, 304, 30)
# index = 0
# while index < 30 :
#     S7.WriteReal(DB_INDEX, 306 + index * 16, 520.6 + index)
#     S7.WriteReal(DB_INDEX, 310 + index * 16, 515.9 + index)
#     S7.WriteReal(DB_INDEX, 314 + index * 16, 0.99)
#     S7.WriteReal(DB_INDEX, 318 + index * 16, 1036.5 + 2 * index)
#     index = index + 1
# S7.WriteBit(DB_INDEX, 250, 0, True)
# S7.WriteShort(DB_INDEX,252,2)
# S7.WriteString(DB_INDEX, 254, 38,'111110223')
# time.sleep(10)
# S7.WriteBit(DB_INDEX, 250, 0, False)

# #请求称重
# S7.WriteShort(DB_INDEX, 812, 2024)
# S7.WriteShort(DB_INDEX, 814, 10)
# S7.WriteShort(DB_INDEX, 816, 16)
# S7.WriteShort(DB_INDEX, 818, 19)
# S7.WriteShort(DB_INDEX, 820, 20)
# S7.WriteShort(DB_INDEX, 822, 30)

# indexWeigh = 0
# while indexWeigh < 10 :
#     S7.WriteReal(DB_INDEX, 824 + indexWeigh * 4, 690.6 + indexWeigh)
#     indexWeigh = indexWeigh + 1

# S7.WriteBit(DB_INDEX, 810, 0, True)
# time.sleep(10)
# S7.WriteBit(DB_INDEX, 810, 0, False)