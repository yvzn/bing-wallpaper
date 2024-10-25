/*
   Copyright 2021-2024 Yvan Razafindramanana

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

	   http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

import React from "react";
import PropTypes from "prop-types";

import getWallpapers, { numberOfImages } from "./service";

const initialState = {
	images: new Array(numberOfImages).fill({}),
	status: "loading",
};

function App() {
	const [state, setState] = React.useState(initialState);

	React.useEffect(() => {
		getWallpapers().then((images) => {
			setState((previousState) => {
				return {
					...previousState,
					images,
					status: "loaded",
				};
			});
		});
	}, []);

	return (
		<>
			<Wallpapers images={state.images} status={state.status} />
		</>
	);
}

export default App;

function Wallpapers({ images, status }) {
	return (
		<>
			{images.map((image, index) => (
				<WallpaperCard key={index} image={image} status={status} />
			))}
		</>
	);
}

const cachedImageProps = PropTypes.shape({
	title: PropTypes.string,
	copyright: PropTypes.string,
	market: PropTypes.string,
	fullResolution: PropTypes.string,
	lowResolution: PropTypes.string,
});

const loadingStatus = PropTypes.oneOf(["loading", "loaded"]);

Wallpapers.propTypes = {
	images: PropTypes.arrayOf(
		PropTypes.shape({
			image: cachedImageProps,
		}),
	).isRequired,
	status: loadingStatus.isRequired,
};

function WallpaperCard({ image, status }) {
	let lang = undefined;
	if (Boolean(image.market) && !image.market.startsWith("en")) {
		lang = image.market;
	}

	return (
		<article lang={lang}>
			<ImageTitle image={image} status={status} />
			<ImagePreview image={image} status={status} />
		</article>
	);
}

WallpaperCard.propTypes = {
	image: cachedImageProps,
	status: loadingStatus.isRequired,
};

function ImagePreview({ image, status }) {
	return (
		<p>
			{status === "loading" && <Skeleton />}
			{status === "loaded" && (
				<a href={image.fullResolution} rel="noreferrer noopener">
					<img
						src={image.lowResolution}
						alt={image.title}
						width="320"
						height="180"
					/>
				</a>
			)}
		</p>
	);
}

ImagePreview.propTypes = {
	image: cachedImageProps,
	status: loadingStatus.isRequired,
};

function ImageTitle({ image, status }) {
	let tooltip =
		Boolean(image.title) && image.title.length > 15
			? image.title
			: undefined;

	return (
		<h2>
			{status === "loading" && <Skeleton>Loading...</Skeleton>}
			{status === "loaded" && <b title={tooltip}>{image.title}</b>}
		</h2>
	);
}

ImageTitle.propTypes = {
	image: cachedImageProps,
	status: loadingStatus.isRequired,
};

function Skeleton({ children }) {
	return <progress max="1">{children}</progress>;
}

Skeleton.propTypes = {
	children: PropTypes.oneOfType([PropTypes.string, PropTypes.element]),
};
