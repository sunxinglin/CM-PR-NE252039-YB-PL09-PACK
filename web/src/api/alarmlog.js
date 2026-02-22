import request from '@/utils/request'

export function getList(data) {
    return request({
        url: '/Alarm/GetList',
        method: 'post',
        data
    })
}
export function GetOutPut(data) {
    return request({
        url: '/Proc_Product_OffLine/GetOutPut',
        method: 'get',
        data
    })
}

export function GetIndexStatistics(params) {
    return request({
        url: '/Proc_Product_OffLine/GetIndexStatistics',
        method: 'get',
        params
    })
}

export function GetPageList(params) {
    return request({
        url: '/Alarm/GetPageList',
        method: 'get',
        params
    })
}