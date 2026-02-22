
# -*- coding: UTF-8 -*-
# define constants

import time


DB_INDEX= 200

# 发送请求

# #请求小车进站
# S7.WriteBit(DB_INDEX, 114,0, True)
# S7.WriteShort(DB_INDEX,116,1)
# S7.WriteString(DB_INDEX, 118, 40,'111110222')
# time.sleep(10)
# S7.WriteBit(DB_INDEX, 114, 0, False)

# # 请求开始拧紧
# S7.WriteBit(DB_INDEX, 180,0, True)
# S7.WriteShort(DB_INDEX,182,1)
# S7.WriteString(DB_INDEX, 184, 40,'111110222')
# time.sleep(10)
# S7.WriteBit(DB_INDEX, 180, 0, False)

###### 请求拧紧完成
index = 0;
while index < 10 :
    if index%2 == 0 :
        S7.WriteBit(DB_INDEX,294 + 44* index ,0,True)
        S7.WriteBit(DB_INDEX,294 + 44* index ,1,False)
    else :
        S7.WriteBit(DB_INDEX,294 + 44* index ,0,False)
        S7.WriteBit(DB_INDEX,294 + 44* index ,1,True)
    S7.WriteShort(DB_INDEX,296 + 44* index, 10)
    S7.WriteReal(DB_INDEX, 298 + 44 * index, 9.8 + 0.01 * index)
    S7.WriteReal(DB_INDEX,306 + 44* index, 100 + 1 * index)
    S7.WriteReal(DB_INDEX,314 + 44 * index, 9)
    S7.WriteReal(DB_INDEX,322 + 44 * index, 11)
    S7.WriteReal(DB_INDEX,326 + 44 * index, 90)
    S7.WriteReal(DB_INDEX,334 + 44 * index, 130)
    index = index + 1

S7.WriteBit(DB_INDEX, 250,0, True)
S7.WriteShort(DB_INDEX,252,1)
S7.WriteString(DB_INDEX, 254, 40,'111110222')
time.sleep(10)
S7.WriteBit(DB_INDEX, 250, 0, False)

