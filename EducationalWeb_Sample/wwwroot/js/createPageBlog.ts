namespace Educational {
    export class CreatePageBlog {
        public static UploadImage(): void {
            const content = document.getElementById("image-modal-content")!;
            content.innerHTML = this.UploadControl();
            const spinner = <HTMLDivElement>content.querySelector('div[class="spinner-border spinner-border-sm"]')!;
            const svgIMG = content.querySelector('svg')!;


            const input = <HTMLInputElement>document.getElementById('formFile');

            // Event handler executed when a file is selected
            //const onSelectFile = () => this.upload(input.files[0]);
            //input.addEventListener('change', onSelectFile, false);
            const button = <HTMLButtonElement>document.getElementById("image-modal-upload");

            button.addEventListener('click', (e) => {
                e.preventDefault();
                e.stopPropagation();

                spinner.style.display = "inline-block";
                svgIMG.style.display = "none";
                content.querySelector("fieldset")!.setAttribute("disabled", "disabled");

                if (this.bindDataToImage(input)) {
                    this.upload(input.files![0]);
                }
            })
        }

        private static bindDataToImage(input): boolean
        {     
            if (input.files[0].size >= 1048576) {
                alert("File size must be under 1 MB");
                (<HTMLButtonElement>document.getElementById("image-modal-close")).click();
                return false;
            }

            const img = (<HTMLImageElement>document.getElementById("uploadedImage"));

            if (input == null) {
                img.src = "/img/placeholder.jpg";
                return false;
            }

            var reader = new FileReader();
            reader.onload = function (e) {
               img.src = <string>e.target.result;              
            };
            reader.readAsDataURL(input.files[0]);

            return true;
        }

        private static upload(file) {

            var fd = new FormData();
            fd.append('image', file);

            fetch('/BlogAdmin/Upload', { // Your POST endpoint
                method: 'POST',
                //headers: {
                //    // Content-Type may need to be completely **omitted**
                //    // or you may need something
                //    "Content-Type": "application/json"
                //},
                body: fd // This is your file object
            }).then(
                
                async response => {

                    let t = await response.text();
                   
                    if (response.status == 200)
                        return t;                   

                    throw new Error(t);
                        
                }                    

            ).then(
                success => {
                    console.log(success); // Handle the success response object
                    this.UpdateImageControl(success);
                }
            ).catch(
               
                error => {
                    error => console.log(error) // Handle the error response object
                    alert(error.message);
                    this.bindDataToImage(null);
                } // Handle the error response object
            );
        }


        private static UpdateImageControl(fileName: string): void {
            // (<HTMLImageElement>document.getElementById("uploadedImage")).src = `/temp/img/${fileName}`;
            (<HTMLInputElement>document.getElementById("imageURL")).value = fileName;
            (<HTMLButtonElement>document.getElementById("image-modal-close")).click();
        }

        private static UploadControl(): string {
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
            `
        }

        public static delete(name: string, id: number) {
            (<HTMLDivElement>document.getElementById("blog_delete_data")).innerHTML = this.deleteControl(name, id);
        }

        public static deleteAction(id: number) {
            this.deleteItem(`/blogAdmin/delete/${id}`).then(data => {
                this.updateDeleteData(id);
            });
        }

        public static updateDeleteData(id: number) {
            const child = document.getElementById(`r_${id}`);
            child?.remove();
            document.getElementById(`xbutton_${id}`)?.click();
        }

        private static deleteControl(name: string, id: number): string {
            return `
            <div class="modal-body text-center d-flex justify-content-between align-items-center">
                <h4 class="fw-light m-0">
                    Do you really want to delete <strong><q>${name}</q></strong>?
                </h4>
                <button type="button" class="btn-close ms-auto" data-bs-dismiss="modal" aria-label="" id="xbutton_${id}">
                </button>
            </div>
            <div class="modal-footer d-flex justify-content-center">
                <button type="submit" class="btn btn-sm btn-danger" onclick="Educational.CreatePageBlog.deleteAction(${id})">
                    Yes
                </button>
                <button type="button" href="#" class="btn btn-sm btn-warning" data-bs-dismiss="modal" aria-label="Close">
                    No, go back
                </button>
            </div>`
        }

        private static async deleteItem(url: string): Promise<any> {
            const response = await fetch(url, {
                method: 'DELETE',
                headers: {
                    'Content-type': 'application/json'
                }
            });

            if (!response.ok) {
                throw new Error(`Response status: ${response.status}`);
            }

            const text = await response.text();
            return text;
        }
    }
}