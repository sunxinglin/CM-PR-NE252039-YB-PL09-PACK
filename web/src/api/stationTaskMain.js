import request from '@/utils/request'

export function GetList(params) {
    return request({
        url: '/StationTaskMain/GetList',
        method: 'get',
        params
    })
}