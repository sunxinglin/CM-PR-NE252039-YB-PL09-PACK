import request from '@/utils/request'

export function Load(data) {
    return request({
        url: '/Proc_LowerBoxGlueInfos/Load',
        method: 'post',
        data
    })

}

//����Pack BOm�ļ�
export function modelExpornt(data) {
    return request({
        url: '/Proc_LowerBoxGlueInfos/ModelExpornt',
        method: 'post',
        data,
        responseType: "blob"
    })
}

