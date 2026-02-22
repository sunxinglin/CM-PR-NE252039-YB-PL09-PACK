import request from '@/utils/request'

export function GetPageList(params) {
    return request({
        url: '/ProcModuleInBoxAGVReqArrived/GetPageList',
        method: 'get',
        params
    })
}

export function Remove(data) {
    return request({
        url: '/ProcModuleInBoxAGVReqArrived/Remove',
        method: 'post',
        data
    })
}
