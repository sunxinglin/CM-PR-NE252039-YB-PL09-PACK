import request from '@/utils/request'

export function GetPageList(params) {
    return request({
        url: '/ProcModuleInBoxRecord/GetPageList',
        method: 'get',
        params
    })
}

export function Delete(data) {
    return request({
        url: '/ProcModuleInBoxRecord/Delete',
        method: 'post',
        data
    })
}
