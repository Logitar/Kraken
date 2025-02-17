import type { App } from "vue";
import { FontAwesomeIcon } from "@fortawesome/vue-fontawesome";

import { library } from "@fortawesome/fontawesome-svg-core";
import { faArrowRightToBracket, faUser, faVial } from "@fortawesome/free-solid-svg-icons";

library.add(faArrowRightToBracket, faUser, faVial);

export default function (app: App) {
  app.component("font-awesome-icon", FontAwesomeIcon);
}
