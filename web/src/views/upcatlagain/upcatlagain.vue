<template>
    <div>
        <div class="app-container">
            <el-card shadow="never" class="boby-small" style="height: 100%">
                <div slot="header" class="clearfix">
                    <span>自动站数据重传</span>
                </div>
                <div>
                    <el-row :gutter="2">
                        <el-col :span="21">
                            <el-input prefix-icon="el-icon-search" size="small" style="width: 200px;height: 20px;"
                                class="filter-item" :placeholder="'pack码/箱体码'" v-model="query.PackPN">
                            </el-input>
                            <el-select v-model="query.Code" placeholder="请选择">
                                <el-option v-for="item in typeoptions" :key="item.id" :label="item.name"
                                    :value="item.id">
                                </el-option>
                            </el-select>
                            <el-button type="primary" icon="el-icon-plus" size="small" @click="upgluedata">
                                重传
                            </el-button>
                        </el-col>
                    </el-row>
                </div>
            </el-card>
        </div>
    </div>
</template>
<script>
import waves from "@/directive/waves"; // 水波纹指令
import Sticky from "@/components/Sticky";
import permissionBtn from "@/components/PermissionBtn";
import Pagination from "@/components/Pagination";
import elDragDialog from "@/directive/el-dragDialog";
import * as automicstationdataupcatlagain from "@/api/automicstationdataupcatlagain";
export default {
    components: {
        Sticky,
        permissionBtn,
        Pagination,
    },
    directives: {
        waves,
        elDragDialog,
    },
    // mounted() {
    //     let h = document.documentElement.clientHeight;
    //     let topH = this.$refs.detaillist.$el.offsetTop;
    //     this.tablegeight = (h - topH) * 0.81;
    //     //this.handleClickClose();
    // },
    data() {
        return {
          
            query: {
               
                PackPN: "",
                Code: "",
            },
         
            typeoptions: [
                {
                    id: "OP120",
                    name: "下箱体涂胶",
                },
                {
                    id: "OP130",
                    name: "Block入箱",
                },
                {
                    id: "OP170",
                    name: "间隙涂胶",
                },
                {
                    id: "OP240",
                    name: "肩部涂胶",
                },
                {
                    id: "OP260",
                    name: "压条加压",
                },
                {
                    id: "OP150",
                    name: "Pack加压",
                },
                {
                    id: "OP150_1",
                    name: "Pack加压1",
                },
            ],
        };
    },
    methods: {
        upgluedata() {
            console.log("请求"+this.query);
            automicstationdataupcatlagain
            .upCatlAgain({ PackPN: this.query.PackPN, Code: this.query.Code })
            .then((response) => {
                if (response.isError) {
                    this.$message({
                        message: "错误代码:" + response.errorCode + "\r\n" + "错误信息:" + response.errorMessage,
                        type: "error",
                    });
                    return;
                }
                this.$message({
                        message: "CATLMES返回正常",
                        type: "success",
                    });
                    return;
              
            });
        },
    },
};
</script>