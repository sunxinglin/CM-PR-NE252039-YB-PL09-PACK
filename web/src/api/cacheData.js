import request from '@/utils/request'

export function GetList(params) {
    return request({
        url: '/ProcModuleInBoxCacheData/GetList',
        method: 'get',
        params
    })
}

export function PutModuleIn(data) {
    return request({
        url: '/ProcModuleInBoxCacheData/PutModule',
        method: 'post',
        data
    })
}

export function TakeOutModule(data) {
    return request({
        url: '/ProcModuleInBoxCacheData/TakeOutModule',
        method: 'post',
        data
    })
}

export function GetListByLineNo(params) {
    return request({
        url: '/ProcModuleInBoxCacheData/GetListByLineNo',
        method: 'get',
        params
    })
}
export function Update(data) {
    return request({
        url: '/ProcModuleInBoxCacheData/Update',
        method: 'post',
        data
    })
}