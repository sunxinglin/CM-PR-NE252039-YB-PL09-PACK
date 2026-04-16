from s7svrsim import hints

def main(ctx :hints.S7Context):
    S7 = ctx.DBService  
    DB_INDEX= 200

    S7.WriteString(DB_INDEX, 102, 24, "001PBCXX000002F9J0100001")

    S7.WriteBit(DB_INDEX, 100, 0, True)
