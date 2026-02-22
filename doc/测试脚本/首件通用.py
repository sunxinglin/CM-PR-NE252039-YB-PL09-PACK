
# -*- coding: UTF-8 -*-
# define constants

import time


DB_INDEX= 200

# 发送请求

# 请求小车进站
S7.WriteBit(DB_INDEX, 204, 3, True)
S7.WriteReal(DB_INDEX,2862,1.23)
S7.WriteReal(DB_INDEX,2866,2.23)
S7.WriteReal(DB_INDEX,2870,3.23)
S7.WriteReal(DB_INDEX,2874,4.23)
S7.WriteReal(DB_INDEX,2878,5.23)
S7.WriteReal(DB_INDEX,2882,6.23)