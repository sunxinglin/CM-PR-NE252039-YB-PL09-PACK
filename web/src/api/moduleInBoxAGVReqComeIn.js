import request from '@/utils/request'

export function GetPageList(params) {
    return request({
        url: '/ProcModuleInBoxAGVReqComeIn/GetPageList',
        method: 'get',
        params
    })
}

export function Remove(data) {
    return request({
        url: '/ProcModuleInBoxAGVReqComeIn/Remove',
        method: 'post',
        data
    })
}
