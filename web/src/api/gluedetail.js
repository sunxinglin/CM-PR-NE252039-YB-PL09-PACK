import request from '@/utils/request'

export function getGlueList(data) {
    return request({
        url: '/StationTask_GlueDetail/GetGlueDetail',
        method: 'post',
        data
    })

}


export function getRealTimeGlueList(data) {
    return request({
        url: '/StationTask_GlueDetail/GetRealTimeGlueDetail',
        method: 'post',
        data
    })

}



export function deleteGluingInfo(data) {
    return request({
        url: '/StationTask_GlueDetail/DeleteGluingInfo',
        method: 'post',
        data
    })
}

//����Pack BOm�ļ�
export function modelExpornt(data) {
    return request({
        url: '/StationTask_GlueDetail/ModelExpornt',
        method: 'post',
        data,
        responseType: "blob"
    })
}

export function UpGluingTime(data) {
    return request({
        url: '/StationTask_GlueDetail/UpGluingTime',
        method: 'post',
        data,
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

export function upCatlAgain(data) {
    return request({
        url: '/AutoGlue/UploadGlueDataAgain',
        method: 'post',
        data
    })
}