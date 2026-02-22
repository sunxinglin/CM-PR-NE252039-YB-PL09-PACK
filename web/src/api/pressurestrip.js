import request from '@/utils/request'

export function Load(data) {
    return request({
        url: '/Proc_PressureInfos/Load',
        method: 'post',
        data
    })

}

export function modelExpornt(data) {
    return request({
        url: '/Proc_PressureInfos/ModelExpornt',
        method: 'post',
        data,
        responseType: "blob"
    })
}

