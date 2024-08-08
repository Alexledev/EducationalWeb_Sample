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
    class DataClient {
        static GetCount(url) {
            return __awaiter(this, void 0, void 0, function* () {
                return yield fetch(url).then((response) => __awaiter(this, void 0, void 0, function* () {
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
    }
    class ControlData {
        courseFilterControl(input) {
            return ` <a href="#" class="list-group-item list-group-item-action d-flex justify-content-between align-items-center">
                            ${input.key}
                            <span class="badge text-bg-warning rounded-pill">${input.value}</span>
                        </a>`;
        }
        bindDataToControl(url, container) {
            const bindPromise = new Promise((resolve, reject) => {
                let controlCollection = [];
                DataClient.GetCount(url).then(data => {
                    let d = data;
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
    class Topics extends ControlData {
        bindData() {
            const container = document.getElementById("courseTopics");
            this.bindDataToControl("/Courses/courseByTopic", container);
        }
    }
    Courses.Topics = Topics;
    class Category extends ControlData {
        bindData(callback) {
            const container = document.getElementById("courseCategories");
            this.bindDataToControl("/Courses/courseByCategory", container).then(callback.bind(this));
        }
    }
    Courses.Category = Category;
})(Courses || (Courses = {}));
let topic = new Courses.Topics();
let category = new Courses.Category();
category.bindData(() => {
    topic.bindData();
});
//# sourceMappingURL=courses.js.map