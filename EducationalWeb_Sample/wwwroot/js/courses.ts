namespace Courses {
    class Pair {
        public key: any;
        public value: any;
    }

    class DataClient {

        public static async GetCount(url: string): Promise<any> {

            return await fetch(url).then(

                async response => {

                    if (response.status != 200) {

                        let message = await response.text();
                        throw new Error(message);
                    }

                    return await response.json();
                }
            ).catch(

                error => {
                    error => console.log(error) // Handle the error response object
                    alert(error.message);
                }
            );
        }
    }

    class ControlData {

        protected courseFilterControl(input: Pair): string {

            return ` <a href="#" class="list-group-item list-group-item-action d-flex justify-content-between align-items-center">
                            ${input.key}
                            <span class="badge text-bg-warning rounded-pill">${input.value}</span>
                        </a>`
        }

        protected bindDataToControl(url: string, container: HTMLDivElement): Promise<any> {

            const bindPromise: Promise<any> = new Promise((resolve, reject) => {

                let controlCollection: string[] = [];

                DataClient.GetCount(url).then(data => {

                    let d = data as Pair[];
                    for (let m of d) {
                        controlCollection.push(this.courseFilterControl(m));
                    }
                    container.innerHTML = controlCollection.join("");

                    resolve(data);
                   
                }).catch((reason) => {
                    reject(reason);
                });     
            });

            return bindPromise;
        }
    }

    export class Topics extends ControlData {
        
        public bindData() {
          
            const container = <HTMLDivElement>document.getElementById("courseTopics");
            this.bindDataToControl("/Courses/courseByTopic", container);               
        }
    }

    export class Category extends ControlData {

        public bindData(callback: Function) {       
            
            const container = <HTMLDivElement>document.getElementById("courseCategories");
            this.bindDataToControl("/Courses/courseByCategory", container).then(callback.bind(this));          
        }
    }
}

let topic = new Courses.Topics();

let category = new Courses.Category();
category.bindData(() => {
    topic.bindData();
});
