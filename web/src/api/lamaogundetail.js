import request from '@/utils/request'

export function Load(data) {
    return request({
        url: '/Proc_StationTask_LaMaoGunDetail/Load',
        method: 'post',
        data
    })

}

//����Pack BOm�ļ�
export function modelExpornt(data) {
    return request({
        url: '/Proc_StationTask_LaMaoGunDetail/ModelExpornt',
        method: 'post',
        data,
        responseType: "blob"
    })
}

