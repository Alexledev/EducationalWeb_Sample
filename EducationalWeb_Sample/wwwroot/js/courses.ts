namespace Courses {
    class Pair {
        public key: any;
        public value: any;
    }

    export class Topics {

        public static async GetTopicCount(): Promise<any> {

            return await fetch("/Courses/courseByTopic").then(

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

        public static bindDataToTopicControl() {
            let controlCollection: string[] = [];

            const container = <HTMLDivElement>document.getElementById("courseTopics");

            this.GetTopicCount().then(data => {

                let d = data as Pair[];
                for (let m of d) {
                    controlCollection.push(this.courseTopicControl(m));
                }

                container.innerHTML = controlCollection.join("");

            });            
        }

        private static courseTopicControl(input: Pair): string {
            
            return ` <a href="#" class="list-group-item list-group-item-action d-flex justify-content-between align-items-center">
                            ${input.key}
                            <span class="badge text-bg-warning rounded-pill">${input.value}</span>
                        </a>`
        }
    }
}

Courses.Topics.bindDataToTopicControl();