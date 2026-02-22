import request from '@/utils/request'

export function GetPageList(params) {
    return request({
        url: '/ProcModuleInBoxTaskRecord/GetPageList',
        method: 'get',
        params
    })
}

export function Remove(data) {
    return request({
        url: '/ProcModuleInBoxTaskRecord/Remove',
        method: 'post',
        data
    })
}
