import request from '@/utils/request'

export function Load(data) {
    return request({
        url: '/Proc_PressureInfos/Load',
        method: 'post',
        data
    })

}

//����Pack BOm�ļ�
export function modelExpornt(data) {
    return request({
        url: '/Proc_PressureInfos/ModelExpornt',
        method: 'post',
        data,
        responseType: "blob"
    })
}

export function LoadPressureData(params) {
    return request({
        url: '/AutoPressure/LoadPressureData',
        method: 'get',
        params
    })

}

export function LoadPressureDataDetail(params) {
    return request({
        url: '/AutoPressure/LoadPressureDataDetail',
        method: 'get',
        params
    })

}



export function upCatlAgain(params) {
    return request({
        url: '/AutoPressure/UploadPressureDataAgain',
        method: 'get',
        params
    })

}