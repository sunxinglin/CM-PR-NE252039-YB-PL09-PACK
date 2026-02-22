# -*- coding: UTF-8 -*-
# define constants

import time

DB_INDEX= 200

# # # 请求小车进站
# # S7.WriteBit(DB_INDEX, 114, 0, True)
# # S7.WriteShort(DB_INDEX,116,1)
# # S7.WriteString(DB_INDEX, 118, 38,'111110222')
# # time.sleep(10)
# # S7.WriteBit(DB_INDEX, 114, 0, False)

# # # 请求开始涂胶
# S7.WriteBit(DB_INDEX, 180, 0, True)
# S7.WriteShort(DB_INDEX,182,1)
# S7.WriteString(DB_INDEX, 184, 38,'111110222')
# time.sleep(10)
# S7.WriteBit(DB_INDEX, 180, 0, False)

# #请求加压完成
S7.WriteShort(DB_INDEX, 294, 2024)
S7.WriteShort(DB_INDEX, 296, 10)
S7.WriteShort(DB_INDEX, 298, 15)
S7.WriteShort(DB_INDEX, 300, 19)
S7.WriteShort(DB_INDEX, 302, 50)
S7.WriteShort(DB_INDEX, 304, 30)
S7.WriteReal(DB_INDEX, 306 , 520.6 )
S7.WriteReal(DB_INDEX, 310 , 520.6 )
S7.WriteReal(DB_INDEX, 314 , 520.6 )
S7.WriteReal(DB_INDEX, 318 , 520.6 )
S7.WriteReal(DB_INDEX, 322 , 520.6 )
S7.WriteReal(DB_INDEX, 326 , 520.6 )
S7.WriteReal(DB_INDEX, 330 , 520.6 )


S7.WriteBit(DB_INDEX, 250, 0, True)
S7.WriteShort(DB_INDEX,252,1)
S7.WriteString(DB_INDEX, 254, 38,'111110222')
time.sleep(10)
S7.WriteBit(DB_INDEX, 250, 0, False)

