import request from '@/utils/request'

export function Load(data) {
    return request({
        url: '/Proc_BeamGlueInfos/Load',
        method: 'post',
        data
    })

}

//����Pack BOm�ļ�
export function modelExpornt(data) {
    return request({
        url: '/Proc_BeamGlueInfos/ModelExpornt',
        method: 'post',
        data,
        responseType: "blob"
    })
}