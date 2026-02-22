import request from '@/utils/request'

export function GetPageList(params) {
    return request({
        url: '/ProcCheckPowerRecord/GetPageList',
        method: 'get',
        params
    })
}