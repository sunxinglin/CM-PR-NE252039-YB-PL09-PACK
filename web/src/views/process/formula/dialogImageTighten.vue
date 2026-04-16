<template>
  <div>
    <div class="app-container">
      <el-col :span="24">
        <el-card shadow="never" class="boby-small" style="height: 100%">
          <div slot="header" class="clearfix">
            <span>图示拧紧任务详情</span>
          </div>
          <div style="margin-bottom: 10px">
            <el-row :gutter="4">
              <el-col :span="20">
                <el-button type="warning" icon="el-icon-back" size="mini" @click="back">返回</el-button>
                <el-button type="primary" icon="el-icon-plus" size="mini" @click="handledpAdd">添加</el-button>
                <el-button type="primary" icon="el-icon-edit" size="mini" @click="handledpEdit">编辑</el-button>
                <el-button type="primary" icon="el-icon-delete" size="mini" @click="handledpDelete">删除</el-button>
                <el-button type="text" size="mini" @click="showAdvanced = !showAdvanced">
                  {{ showAdvanced ? "隐藏高级参数" : "显示高级参数" }}
                </el-button>
              </el-col>
            </el-row>
          </div>
          <div>
            <el-table :data="dpData" ref="dpTable" row-key="id" @current-change="handleSelectiondpChange" border fit
              stripe highlight-current-row align="left">
              <el-table-column property="taskName" label="任务名称" align="center" width="160"></el-table-column>
              <el-table-column property="screwNum" align="center" label="螺栓总量"></el-table-column>
              <el-table-column property="programNo" align="center" label="程序号"></el-table-column>
              <el-table-column property="devicesNos" label="枪号" align="center"></el-table-column>
              <template v-if="showAdvanced">
                <el-table-column property="minTorque" label="最小扭矩" align="center"></el-table-column>
                <el-table-column property="maxTorque" label="最大扭矩" align="center"></el-table-column>
                <el-table-column property="minAngle" label="最小角度" align="center"></el-table-column>
                <el-table-column property="maxAngle" label="最大角度" align="center"></el-table-column>
              </template>
              <el-table-column property="upMesCode" label="上传代码" align="center"></el-table-column>
              <el-table-column label="操作" min-width="240" align="center" class-name="image-tighten-op-col">
                <template slot-scope="scope">
                  <el-button size="mini" @click="openUploadImage(scope.row)" type="primary">上传图片</el-button>
                  <el-button size="mini" @click="viewImage(scope.row)" type="primary">查看图片</el-button>
                  <el-button size="mini" @click="viewPoints(scope.row)" type="primary">查看点位</el-button>
                </template>
              </el-table-column>
            </el-table>
          </div>
        </el-card>
      </el-col>
    </div>
    <el-dialog v-el-drag-dialog class="dialog-mini" width="600px" :title="textMap[dialogStatus]"
      :visible.sync="dialogBNVisible">
      <div>
        <el-form :rules="anyloadRules" ref="dpForm" :model="dpTemp" label-position="right" label-width="100px">
       
          <el-form-item size="small" :label="'任务名称'" prop="screwSpecs">
            <el-input v-model="dpTemp.taskName"></el-input>
          </el-form-item>
          <el-form-item size="small" :label="'螺栓总量'" prop="screwNum">
            <el-input v-model="dpTemp.screwNum"></el-input>
          </el-form-item>

          <el-form-item size="small" :label="'程序号'" prop="programNo">
            <el-input v-model="dpTemp.programNo"></el-input>
          </el-form-item>
          <el-form-item size="small" :label="'枪号'" prop="devicesNos">
            <el-input v-model="dpTemp.devicesNos"></el-input>
          </el-form-item>
          <el-collapse v-model="editCollapseActive">
            <el-collapse-item title="高级参数" name="advanced">
              <el-form-item size="small" :label="'最小扭矩'" prop="torqueMinLimit">
                <el-input v-model="dpTemp.minTorque"></el-input>
              </el-form-item>
              <el-form-item size="small" :label="'最大扭矩'" prop="torqueMaxLimit">
                <el-input v-model="dpTemp.maxTorque"></el-input>
              </el-form-item>
              <el-form-item size="small" :label="'最小角度'" prop="angleMinLimit">
                <el-input v-model="dpTemp.minAngle"></el-input>
              </el-form-item>
              <el-form-item size="small" :label="'最大角度'" prop="angleMaxLimit">
                <el-input v-model="dpTemp.maxAngle"></el-input>
              </el-form-item>
            </el-collapse-item>
          </el-collapse>
          <el-form-item size="small" :label="'上传代码'" prop="upMesCode">
            <el-input v-model="dpTemp.upMesCode"></el-input>
          </el-form-item>
        </el-form>
      </div>
      <div slot="footer">
        <el-button size="mini" @click="dialogBNVisible = false">取消</el-button>
        <el-button size="mini" v-if="dialogStatus == 'create'" type="primary" @click="createDpData">确认</el-button>
        <el-button size="mini" v-else type="primary" @click="updateDpData">确认</el-button>
      </div>
    </el-dialog>
    <!-- 电批弹框2-->

    <el-dialog v-el-drag-dialog class="dialog-mini" width="900px" title="查看图片" :visible.sync="imageDialogVisible">
      <div style="text-align: center;">
        <img v-if="imagePreviewUrl" :src="imagePreviewUrl" style="max-width: 100%; max-height: 70vh;" />
      </div>
    </el-dialog>

    <el-dialog v-el-drag-dialog class="dialog-mini" width="1000px" title="查看点位" :visible.sync="pointsDialogVisible">
      <div class="layout-container">
        <div class="layout-image">
          <img ref="layoutImg" v-if="layoutImagePreviewUrl" :src="layoutImagePreviewUrl" @load="onLayoutImageLoaded"
            style="max-width: 100%; max-height: 70vh;" />
          <div v-for="p in layoutPoints" :key="p.orderNo" class="layout-point" :style="getPointStyle(p)">
            {{ p.orderNo }}
          </div>
        </div>
        <div class="layout-table">
          <el-table :data="layoutPoints" border fit stripe height="300">
            <el-table-column prop="orderNo" label="序号" width="80" align="center" />
            <el-table-column prop="point_X" label="X" align="center" />
            <el-table-column prop="point_Y" label="Y" align="center" />
          </el-table>
        </div>
      </div>
    </el-dialog>

    <el-dialog v-el-drag-dialog class="dialog-mini" width="600px" title="上传图片" :visible.sync="uploadImageVisible">
      <div>
        <el-upload ref="imageUpload" action="string" :auto-upload="false" :on-change="onImageFileChange" :limit="1"
          :file-list="uploadFileList" drag accept="image/*">
          <i class="el-icon-upload"></i>
          <div class="el-upload__text">将文件拖到此处，或<em>点击选择</em></div>
        </el-upload>
      </div>
      <div slot="footer">
        <el-button size="mini" @click="uploadImageVisible = false">取消</el-button>
        <el-button size="mini" type="primary" :loading="uploadImageLoading" @click="submitUploadImage">上传</el-button>
      </div>
    </el-dialog>
  </div>
</template>

<script>
import * as stationtaskscrewrework from "@/api/stationtasktightenbyimage.js";
import * as proresource from "@/api/proresource.js";

import elDragDialog from "@/directive/el-dragDialog";
import waves from "@/directive/waves"; // 水波纹指令
export default {
  directives: {
    waves,
    elDragDialog,
  },
  data() {
      var checkPositivenumber = (rule, value, callback) => {
      if (rule.field =="useNum") {
        if (Number(value)  < 1) {
        callback(new Error("最小为1"));
      } 
      }
      if (Number(value)  < 0) {
        callback(new Error("最小为0"));
      }
      callback();
    };
    return {
      imageHost: "",
      showAdvanced: false,
      editCollapseActive: [],
      imageDialogVisible: false,
      imagePreviewUrl: "",
      pointsDialogVisible: false,
      layoutImagePreviewUrl: "",
      layoutCanvasWidth: 0,
      layoutCanvasHeight: 0,
      layoutDisplayWidth: 0,
      layoutDisplayHeight: 0,
      layoutPoints: [],
      uploadImageVisible: false,
      uploadImageLoading: false,
      uploadRow: null,
      uploadFileList: [],
      dpTemp: {
        id: undefined,
        stationTaskId:this.taskId,
        taskName:'',
        screwNum:1,
        programNo:1,
        upMesCode:"",
        devicesNos:"1",
        minTorque:0,
        maxTorque:180,
        minAngle:0,
        maxAngle:180,
      },
      textMap: {
        update: "编辑",
        create: "添加",
        detail: "任务详情",
      },
      stationBoltGunList: [], //拧紧枪列表
      stationBoltGunGroupList: [], //拧紧枪列表
      anyloadRules: {
        screwNum: [
          {
            required: true,
            message: "使用数量不能为空",
            trigger: "blur",
            validator: checkPositivenumber,
          },
        ],
        devicesNos: [
          {
            required: true,
            message: "枪号不能为空",
            trigger: "blur",
          
          },
        ],
        programNo: [
          {
            required: true,
            message: "程序号不能为空",
            trigger: "blur",
          
          },
        ],
        upMesCode: [
          {
            required: true,
            message: "上传代码不能为空",
            trigger: "blur",
          },
        ],
      },
      
      dialogStatus: "", //编辑框功能(添加/编辑)
      dpData: [],
      resourceData: [],
      dialogBNVisible: false,
      taskId: 0,
      stepId: 0,
    };
  },
  mounted() {
    this.Load();
    this.loadresource();
    this.imageHost = `${window.location.protocol}//${window.location.hostname}:8128`;
  },
  methods: {
    //#region 拧紧枪

    resetdpdata() {
      this.$refs.dpTable.setCurrentRow();
      this.dpTemp = {
        id: undefined,
        taskName:'',
        stationTaskId:this.taskId,
        screwNum:1,
        programNo:1,
        upMesCode:"",
        devicesNos:"1",
        minTorque:0,
        maxTorque:180,
        minAngle:0,
        maxAngle:180,
      };
    },
    handledpAdd() {
      this.dialogStatus = "create"; //编辑框功能选择（添加）
      this.dialogBNVisible = true; //编辑框显示
      this.$nextTick(() => {
        this.$refs["dpForm"].clearValidate();
      });
      this.resetdpdata();
    },
    handledpEdit() {
      if (this.dpTemp.id != undefined) {
        this.dialogStatus = "update"; //编辑框功能选择（添加）
        this.dialogBNVisible = true; //编辑框显示
        this.$nextTick(() => {
          this.$refs["dpForm"].clearValidate();
        });
      } else {
        this.$message({
          message: "请选择一个想要修改的数据",
          type: "error",
        });
      }
    },
    handledpDelete() {
      if (this.dpTemp.id === undefined) {
        this.$message({
          message: "请选择一个想要删除的数据",
          type: "error",
        });
        return;
      }
      this.$confirm("确定要删除吗？").then((_) => {
        //提取复选框的数据的Id
        var selectids = [];
        selectids.push(this.dpTemp.id); //提取复选框的数据的Id
        var params = {
          ids: selectids,
        };
        stationtaskscrewrework.del(params).then(() => {
          this.$notify({
            title: "成功",
            message: "删除成功",
            type: "success",
            duration: 2000,
          });
          this.Load();
          //页面加载
        });
      });
    },

    handleSelectiondpChange(val) {
      if (val === null) {
        return;
      } else {
        this.dpTemp = val;
      }
    },

    updateDpData() {
      this.$refs["dpForm"].validate((valid) => {
        if (valid) {
          this.dpTemp.reworkLimitTimes=this.dpTemp.useNum;
          stationtaskscrewrework.update(this.dpTemp).then((response) => {
            this.dialogBNVisible = false; //编辑框关闭
            this.$notify({
              title: "成功",
              message: "修改成功",
              type: "success",
              duration: 2000,
            });
            this.Load();
            this.resetdpdata();
          });
        }
      });
    },
    createDpData() {
      this.$refs["dpForm"].validate((valid) => {
        if (valid) {
          this.dpTemp.stationTaskId = this.taskId;
          stationtaskscrewrework.add(this.dpTemp).then((response) => {
            this.dialogBNVisible = false; //编辑框关闭
            this.$notify({
              title: "成功",
              message: "创建成功",
              type: "success",
              duration: 2000,
            });
            this.Load();
            this.resetdpdata();
          });
        }
      });
    },
    Load() {
      if (this.taskId == 0) {
        this.stepId = this.$parent.stepId;
        this.taskId = this.$parent.taskId;
      }
      stationtaskscrewrework.Load({ taskId: this.taskId }).then((response) => {
        this.dpData = response.result; //提取数据表
      });
      this.$nextTick(() => {});
    },
    loadresource() {
      proresource
        .getlistbystepId({ stepId: this.stepId })
        .then((response) => {
          this.resourceData = response.result;
          console.log(this.resourceData);
        });
    },
    back() {
      this.$parent.imageTightenVisible = false;
      this.$parent.taskvisible = true;
      this.taskId = 0;
      this.stepId = 0;
    },
    buildImageUrl(path) {
      if (!path) return "";
      if (typeof path !== "string") return "";
      if (path.indexOf("http://") === 0 || path.indexOf("https://") === 0) return path;
      if (path.indexOf("/") === 0) return `${this.imageHost}${path}`;
      return `${this.imageHost}/${path}`;
    },
    viewImage(row) {
      const tid = row && row.stationTaskId ? row.stationTaskId : this.taskId;
      stationtaskscrewrework.GetImageUrl({ taskId: tid }).then((response) => {
        const p = response && (response.result || response.data || response.url || response);
        const url = this.buildImageUrl(p);
        if (!url) {
          this.$message({
            message: (response && response.message) || "未获取到图片地址",
            type: "error",
          });
          return;
        }
        this.imagePreviewUrl = url;
        this.imageDialogVisible = true;
      });
    },
    viewPoints(row) {
      const tid = row && row.stationTaskId ? row.stationTaskId : this.taskId;
      stationtaskscrewrework.LoadLayout({ taskId: tid }).then((response) => {
        const result = response && response.result ? response.result : null;
        if (!result) {
          this.$message({
            message: (response && response.message) || "未获取到点位信息",
            type: "error",
          });
          return;
        }
        this.layoutImagePreviewUrl = this.buildImageUrl(result.imageUrl);
        const canvas = result.canvasLayout || {};
        this.layoutCanvasWidth = canvas.canvasWidth || 0;
        this.layoutCanvasHeight = canvas.canvasHeight || 0;
        this.layoutPoints = (canvas.points || []).slice().sort((a, b) => a.orderNo - b.orderNo);
        this.pointsDialogVisible = true;
        this.$nextTick(() => {
          this.onLayoutImageLoaded();
        });
      });
    },
    onLayoutImageLoaded() {
      const img = this.$refs.layoutImg;
      if (!img) return;
      this.layoutDisplayWidth = img.clientWidth || 0;
      this.layoutDisplayHeight = img.clientHeight || 0;
    },
    getPointStyle(p) {
      if (!p) return {};
      if (!this.layoutCanvasWidth || !this.layoutCanvasHeight) return { display: "none" };
      if (!this.layoutDisplayWidth || !this.layoutDisplayHeight) return { display: "none" };

      const left = (p.point_X / this.layoutCanvasWidth) * this.layoutDisplayWidth;
      const top = (p.point_Y / this.layoutCanvasHeight) * this.layoutDisplayHeight;
      return {
        left: `${left}px`,
        top: `${top}px`,
      };
    },
    openUploadImage(row) {
      this.uploadRow = row || null;
      this.uploadFileList = [];
      this.uploadImageVisible = true;
      this.$nextTick(() => {
        if (this.$refs.imageUpload) {
          this.$refs.imageUpload.clearFiles();
        }
      });
    },
    onImageFileChange(file, fileList) {
      this.uploadFileList = (fileList || []).slice(-1);
    },
    submitUploadImage() {
      if (!this.uploadFileList || this.uploadFileList.length === 0) {
        this.$message("请选择图片后重试!");
        return;
      }
      const row = this.uploadRow || {};
      const tid = row.stationTaskId ? row.stationTaskId : this.taskId;
      const tname = row.taskName || "";
      const raw = this.uploadFileList[0].raw;
      if (!raw) {
        this.$message("请选择图片后重试!");
        return;
      }

      const form = new FormData();
      form.append("TaskId", tid);
      form.append("TaskName", tname);
      form.append("ImageFile", raw);

      this.uploadImageLoading = true;
      stationtaskscrewrework
        .UploadImage(form)
        .then(() => {
          this.$notify({
            title: "成功",
            message: "上传成功",
            type: "success",
            duration: 2000,
          });
          this.uploadImageVisible = false;
          this.Load();
        })
        .finally(() => {
          this.uploadImageLoading = false;
        });
    },
  },
};
</script>
 
<style>
.image-tighten-op-col .cell {
  display: flex;
  justify-content: center;
  align-items: center;
  gap: 8px;
  white-space: nowrap;
}
.layout-container {
  display: flex;
  flex-direction: column;
  gap: 12px;
}
.layout-image {
  position: relative;
  display: inline-block;
  width: 100%;
  text-align: center;
}
.layout-point {
  position: absolute;
  width: 22px;
  height: 22px;
  line-height: 22px;
  border-radius: 50%;
  background: #f56c6c;
  color: #fff;
  font-size: 12px;
  transform: translate(-50%, -50%);
  pointer-events: none;
}
</style>
