var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
var Educational;
(function (Educational) {
    class CreatePageCourse {
        static UploadImage() {
            const content = document.getElementById("image-modal-content");
            content.innerHTML = this.UploadControl();
            const spinner = content.querySelector('div[class="spinner-border spinner-border-sm"]');
            const svgIMG = content.querySelector('svg');
            const input = document.getElementById('formFile');
            // Event handler executed when a file is selected
            //const onSelectFile = () => this.upload(input.files[0]);
            //input.addEventListener('change', onSelectFile, false);
            const button = document.getElementById("image-modal-upload");
            button.addEventListener('click', (e) => {
                e.preventDefault();
                e.stopPropagation();
                spinner.style.display = "inline-block";
                svgIMG.style.display = "none";
                content.querySelector("fieldset").setAttribute("disabled", "disabled");
                if (this.bindDataToImage(input)) {
                    this.upload(input.files[0]);
                }
            });
        }
        static bindDataToImage(input) {
            if (input.files[0].size >= 1048576) {
                alert("File size must be under 1 MB");
                document.getElementById("image-modal-close").click();
                return false;
            }
            const img = document.getElementById("uploadedImage");
            if (input == null) {
                img.src = "/img/placeholder.jpg";
                return false;
            }
            var reader = new FileReader();
            reader.onload = function (e) {
                img.src = e.target.result;
            };
            reader.readAsDataURL(input.files[0]);
            return true;
        }
        static upload(file) {
            var fd = new FormData();
            fd.append('image', file);
            fetch('/Content/Upload', {
                method: 'POST',
                //headers: {
                //    // Content-Type may need to be completely **omitted**
                //    // or you may need something
                //    "Content-Type": "application/json"
                //},
                body: fd // This is your file object
            }).then((response) => __awaiter(this, void 0, void 0, function* () {
                let t = yield response.text();
                if (response.status == 200)
                    return t;
                throw new Error(t);
            })).then(success => {
                console.log(success); // Handle the success response object
                this.UpdateImageControl(success);
            }).catch(error => {
                error => console.log(error); // Handle the error response object
                alert(error.message);
                this.bindDataToImage(null);
            } // Handle the error response object
            );
        }
        static UpdateImageControl(fileName) {
            // (<HTMLImageElement>document.getElementById("uploadedImage")).src = `/temp/img/${fileName}`;
            document.getElementById("imageURL").value = fileName;
            document.getElementById("image-modal-close").click();
        }
        static UploadControl() {
            return `      
                <fieldset>
                    <div class="row gx-2">
                        <div class="col-md">
                            <div class="mb-2">
                                <input class="form-control" type="file" accept="image/*" name="image" id="formFile">
                            </div>
                        </div>
                        <div class="col-md-12">
                            <div class="d-grid">
                                <button type="button" class="btn btn-success btn-sm" id="image-modal-upload">
                                    <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-upload" viewBox="0 0 16 16">
                                        <path d="M.5 9.9a.5.5 0 0 1 .5.5v2.5a1 1 0 0 0 1 1h12a1 1 0 0 0 1-1v-2.5a.5.5 0 0 1 1 0v2.5a2 2 0 0 1-2 2H2a2 2 0 0 1-2-2v-2.5a.5.5 0 0 1 .5-.5" />
                                        <path d="M7.646 1.146a.5.5 0 0 1 .708 0l3 3a.5.5 0 0 1-.708.708L8.5 2.707V11.5a.5.5 0 0 1-1 0V2.707L5.354 4.854a.5.5 0 1 1-.708-.708z" />
                                    </svg>
                                    <div class="spinner-border spinner-border-sm" role="status" style="display:none">
                                        <span class="visually-hidden">Loading..</span>
                                    </div>
                                    Upload
                                </button>
                            </div>
                        </div>
                    </div>
                </fieldset>
            `;
        }
        static delete(name, id) {
            document.getElementById("course_delete_data").innerHTML = this.deleteControl(name, id);
        }
        static deleteAction(id) {
            this.deleteItem(`/content/delete/${id}`).then(data => {
                this.updateDeleteData(id);
            });
        }
        static updateDeleteData(id) {
            var _a;
            const child = document.getElementById(`r_${id}`);
            child === null || child === void 0 ? void 0 : child.remove();
            (_a = document.getElementById(`xbutton_${id}`)) === null || _a === void 0 ? void 0 : _a.click();
        }
        static deleteControl(name, id) {
            return `
            <div class="modal-body text-center d-flex justify-content-between align-items-center">
                <h4 class="fw-light m-0">
                    Do you really want to delete <strong><q>${name}</q></strong>?
                </h4>
                <button type="button" class="btn-close ms-auto" data-bs-dismiss="modal" aria-label="" id="xbutton_${id}">
                </button>
            </div>
            <div class="modal-footer d-flex justify-content-center">
                <button type="submit" class="btn btn-sm btn-danger" onclick="Educational.CreatePageCourse.deleteAction(${id})">
                    Yes
                </button>
                <button type="button" href="#" class="btn btn-sm btn-warning" data-bs-dismiss="modal" aria-label="Close">
                    No, go back
                </button>
            </div>`;
        }
        static deleteItem(url) {
            return __awaiter(this, void 0, void 0, function* () {
                const response = yield fetch(url, {
                    method: 'DELETE',
                    headers: {
                        'Content-type': 'application/json'
                    }
                });
                if (!response.ok) {
                    throw new Error(`Response status: ${response.status}`);
                }
                const text = yield response.text();
                return text;
            });
        }
    }
    Educational.CreatePageCourse = CreatePageCourse;
})(Educational || (Educational = {}));
//# sourceMappingURL=createPageCourse.js.map