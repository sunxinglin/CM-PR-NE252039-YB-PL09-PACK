# 西门子 S7-1200/1500 PLC 数据类型与 C# 类型及字节大小映射表
# 格式：'PLC_Type': {'csharp_type': 'C# 类型名', 'byte_size': 字节数, 'description': '描述'}

S7_TYPE_MAP = {
    # 基本布尔类型
    "Bool": {
        "csharp_type": "bool",
        "byte_size": 1,  # PLC 中 Bool 占 1 位，但寻址通常按字节处理，实际传输常按字节对齐
        "description": "布尔量 (0 或 1)"
    },
    "Byte": {
        "csharp_type": "byte",
        "byte_size": 1,
        "description": "8 位无符号整数"
    },
    "Char": {
        "csharp_type": "byte", # S7 Char 通常是 ASCII 码，对应 byte
        "byte_size": 1,
        "description": "字符 (ASCII)"
    },
    
    # 整数类型
    "SInt": {
        "csharp_type": "sbyte",
        "byte_size": 1,
        "description": "8 位有符号整数 (-128 到 127)"
    },
    "USInt": {
        "csharp_type": "byte",
        "byte_size": 1,
        "description": "8 位无符号整数 (0 到 255)"
    },
    "Int": {
        "csharp_type": "short",
        "byte_size": 2,
        "description": "16 位有符号整数"
    },
    "UInt": {
        "csharp_type": "ushort",
        "byte_size": 2,
        "description": "16 位无符号整数"
    },
    "DInt": {
        "csharp_type": "int",
        "byte_size": 4,
        "description": "32 位有符号整数"
    },
    "UDInt": {
        "csharp_type": "uint",
        "byte_size": 4,
        "description": "32 位无符号整数"
    },
    "LInt": {
        "csharp_type": "long",
        "byte_size": 8,
        "description": "64 位有符号整数 (S7-1500 支持)"
    },
    "ULInt": {
        "csharp_type": "ulong",
        "byte_size": 8,
        "description": "64 位无符号整数 (S7-1500 支持)"
    },

    # 浮点数类型
    "Real": {
        "csharp_type": "float",
        "byte_size": 4,
        "description": "32 位 IEEE 754 浮点数"
    },
    "LReal": {
        "csharp_type": "double",
        "byte_size": 8,
        "description": "64 位 IEEE 754 浮点数"
    },

    # 时间与日期类型 (通常映射为 long 或 specific struct，这里按底层存储字节标注)
    "Time": {
        "csharp_type": "int", # S7 Time 是 32 位毫秒数
        "byte_size": 4,
        "description": "时间间隔 (毫秒)"
    },
    "LTOD": {
        "csharp_type": "long", # Long Time of Day
        "byte_size": 8,
        "description": "长型时间_of_日 (S7-1500)"
    },
    "Date": {
        "csharp_type": "ushort",
        "byte_size": 2,
        "description": "日期 (自 1990-01-01 的天数)"
    },
    "TOD": {
        "csharp_type": "uint",
        "byte_size": 4,
        "description": "一天中的时间 (毫秒)"
    },
    "DTL": {
        "csharp_type": "byte[]", # 复杂结构，通常 12 字节
        "byte_size": 12,
        "description": "日期和时间长型 (S7-1200/1500 标准)"
    },

    # 字符串类型
    "String": {
        "csharp_type": "byte[]", # S7 String 包含长度头，实际需解析
        "byte_size": 254, # 默认最大长度，实际取决于定义 [254]
        "description": "变长字符串 (最大 254 字节 + 2 字节头)"
    },
    "WString": {
        "csharp_type": "byte[]",
        "byte_size": 54, # 默认示例，实际取决于定义
        "description": "变长宽字符串 (Unicode)"
    },
    
    # 其他
    "Array": {
        "csharp_type": "Array",
        "byte_size": 0, # 取决于元素类型和数量
        "description": "数组，大小由元素类型 * 数量决定"
    }
}

def get_plc_type_info(plc_type_name):
    """
    获取指定 PLC 类型的详细信息
    :param plc_type_name: PLC 类型名称 (如 'Int', 'Real')
    :return: 字典包含 csharp_type, byte_size, description
    """
    return S7_TYPE_MAP.get(plc_type_name, None)




# 常用数据类型字节数常量
SIZE_BOOL = S7_TYPE_MAP["Bool"]["byte_size"]      # 1
SIZE_BYTE = S7_TYPE_MAP["Byte"]["byte_size"]      # 1
SIZE_CHAR = S7_TYPE_MAP["Char"]["byte_size"]      # 1
SIZE_SINT = S7_TYPE_MAP["SInt"]["byte_size"]      # 1
SIZE_USINT = S7_TYPE_MAP["USInt"]["byte_size"]    # 1
SIZE_INT = S7_TYPE_MAP["Int"]["byte_size"]        # 2
SIZE_UINT = S7_TYPE_MAP["UInt"]["byte_size"]      # 2
SIZE_DINT = S7_TYPE_MAP["DInt"]["byte_size"]      # 4
SIZE_UDINT = S7_TYPE_MAP["UDInt"]["byte_size"]    # 4
SIZE_LINT = S7_TYPE_MAP["LInt"]["byte_size"]      # 8
SIZE_ULINT = S7_TYPE_MAP["ULInt"]["byte_size"]    # 8
SIZE_REAL = S7_TYPE_MAP["Real"]["byte_size"]      # 4
SIZE_LREAL = S7_TYPE_MAP["LReal"]["byte_size"]    # 8
SIZE_TIME = S7_TYPE_MAP["Time"]["byte_size"]      # 4
SIZE_DATE = S7_TYPE_MAP["Date"]["byte_size"]      # 2
SIZE_TOD = S7_TYPE_MAP["TOD"]["byte_size"]        # 4
SIZE_DTL = S7_TYPE_MAP["DTL"]["byte_size"]        # 12
