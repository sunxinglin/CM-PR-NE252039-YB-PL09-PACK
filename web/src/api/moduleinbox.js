import request from '@/utils/request'

export function Load(data) {
    return request({
        url: '/Proc_BlockInCaseInfos/Load',
        method: 'post',
        data
    })

}

export function modelExpornt(data) {
    return request({
        url: '/Proc_BlockInCaseInfos/ModelExpornt',
        method: 'post',
        data,
        responseType: "blob"
    })
}

export function LoadData(params) {
    return request({
        url: '/ModuleInBox/LoadData',
        method: 'get',
        params
    })

}

export function LoadDataDetail(params) {
    return request({
        url: '/ModuleInBox/LoadDataDetail',
        method: 'get',
        params
    })

}

export function upCatlAgain(params) {
    return request({
        url: '/ModuleInBox/UploadDataAgain',
        method: 'get',
        params
    })

}
