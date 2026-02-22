import request from '@/utils/request'

export function Load(data) {
    return request({
        url: '/Proc_GluingInfos/Load',
        method: 'post',
        data
    })

}

export function modelExpornt(data) {
    return request({
        url: '/Proc_GluingInfos/ModelExpornt',
        method: 'post',
        data,
        responseType: "blob"
    })
}

export function LoadGlueData(params) {
    return request({
        url: '/AutoGlue/LoadGlueData',
        method: 'get',
        params
    })

}

export function LoadGlueDataDetail(params) {
    return request({
        url: '/AutoGlue/LoadGlueDataDetail',
        method: 'get',
        params
    })

}