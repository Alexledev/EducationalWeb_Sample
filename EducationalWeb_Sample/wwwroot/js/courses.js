var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
var Courses;
(function (Courses) {
    class Pair {
    }
    class Topics {
        static GetTopicCount() {
            return __awaiter(this, void 0, void 0, function* () {
                return yield fetch("/Courses/courseByTopic").then((response) => __awaiter(this, void 0, void 0, function* () {
                    if (response.status != 200) {
                        let message = yield response.text();
                        throw new Error(message);
                    }
                    return yield response.json();
                })).catch(error => {
                    error => console.log(error); // Handle the error response object
                    alert(error.message);
                });
            });
        }
        static bindDataToTopicControl() {
            let controlCollection = [];
            const container = document.getElementById("courseTopics");
            this.GetTopicCount().then(data => {
                let d = data;
                for (let m of d) {
                    controlCollection.push(this.courseTopicControl(m));
                }
                container.innerHTML = controlCollection.join("");
            });
        }
        static courseTopicControl(input) {
            return ` <a href="#" class="list-group-item list-group-item-action d-flex justify-content-between align-items-center">
                            ${input.key}
                            <span class="badge text-bg-warning rounded-pill">${input.value}</span>
                        </a>`;
        }
    }
    Courses.Topics = Topics;
})(Courses || (Courses = {}));
Courses.Topics.bindDataToTopicControl();
//# sourceMappingURL=courses.js.map