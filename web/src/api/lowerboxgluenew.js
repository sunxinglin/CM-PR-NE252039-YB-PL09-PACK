import request from '@/utils/request'

export function UploadDataAgain(data) {
    return request({
        url: '/LowerBoxGlue/UploadDataAgain',
        method: 'post',
        data
    })

}

export function Load(params) {
    return request({
        url: '/LowerBoxGlue/LoadData',
        method: 'get',
        params
    })

}

export function TimeLoad(params) {
    return request({
        url: '/LowerBoxGlue/LoadTimeData',
        method: 'get',
        params
    })

}


export function modelExpornt(data) {
    return request({
        url: '/LowerBoxGlue/ModelExpornt',
        method: 'post',
        data,
        responseType: "blob"
    })
}

