import request from '@/utils/request'

export function GetPageList(params) {
    return request({
        url: '/ProcModuleInCache/GetPageList',
        method: 'get',
        params
    })
}

export function Delete(data) {
    return request({
        url: '/ProcModuleInCache/Delete',
        method: 'post',
        data
    })
}
