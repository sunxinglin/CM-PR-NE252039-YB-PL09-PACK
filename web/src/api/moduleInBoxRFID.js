import request from '@/utils/request'

export function GetPageList(params) {
    return request({
        url: '/ProcModuleInBoxRFID/GetPageList',
        method: 'get',
        params
    })
}

export function Remove(data) {
    return request({
        url: '/ProcModuleInBoxRFID/Remove',
        method: 'post',
        data
    })
}
