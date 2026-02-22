import request from '@/utils/request'

export function GetList(params) {
    return request({
        url: '/StationTaskBomDetail/GetPageList',
        method: 'get',
        params
    })
}
export function modelExpornt(data) {
    return request({
        url: '/StationTaskBomDetail/ModelExpornt',
        method: 'post',
        data,
        responseType: "blob"
    })
}