import request from '@/utils/request'

export function Load(data) {
    return request({
        url: '/AutoGlue/LoadGlueData',
        method: 'get',
        data
    })

}

//����Pack BOm�ļ�
export function modelExpornt(data) {
    return request({
        url: '/AutoGlue/ModelExpornt',
        method: 'post',
        data,
        responseType: "blob"
    })
}