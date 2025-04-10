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
	selectedImageIndex: undefined,
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

	const selectImage = (imageIndex) => {
		setState((previousState) => {
			return {
				...previousState,
				selectedImageIndex: imageIndex,
			};
		});
	};

	const selectedImage =
		state.selectedImageIndex === undefined
			? undefined
			: state.images[state.selectedImageIndex];

	return (
		<>
			<Wallpapers
				images={state.images}
				status={state.status}
				onSelectImage={selectImage}
			/>
			<WallpaperDetails
				image={selectedImage}
				onClose={() => selectImage(undefined)}
			/>
		</>
	);
}

export default App;

function Wallpapers({ images, status, onSelectImage }) {
	const selectWallpaperCard = (index) => (event) => {
		if (event.altKey || event.ctrlKey || event.metaKey || event.shiftKey) {
			window.open(images[index].fullResolution, "_blank");
		} else {
			onSelectImage(index);
		}
	};

	return (
		<>
			{images.map((image, index) => (
				<WallpaperCard
					key={index}
					image={image}
					status={status}
					onClick={selectWallpaperCard(index)}
				/>
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
	ultraHighResolution: PropTypes.string,
});

const loadingStatus = PropTypes.oneOf(["loading", "loaded"]);

Wallpapers.propTypes = {
	images: PropTypes.arrayOf(cachedImageProps).isRequired,
	status: loadingStatus.isRequired,
	onSelectImage: PropTypes.func.isRequired,
};

function WallpaperCard({ image, status, onClick }) {
	return (
		<article>
			<ImageTitle image={image} status={status} />
			<ImagePreview image={image} status={status} onClick={onClick} />
		</article>
	);
}

WallpaperCard.propTypes = {
	image: cachedImageProps,
	status: loadingStatus.isRequired,
	onClick: PropTypes.func.isRequired,
};

function ImagePreview({ image, status, onClick }) {
	const lang = getMarketLanguage(image.market);

	return (
		<p>
			{status === "loading" && <Skeleton />}
			{status === "loaded" && (
				<input
					type="image"
					onClick={onClick}
					src={image.lowResolution}
					alt={image.title}
					width="320"
					height="180"
					lang={lang}
				/>
			)}
		</p>
	);
}

ImagePreview.propTypes = {
	image: cachedImageProps,
	status: loadingStatus.isRequired,
	onClick: PropTypes.func.isRequired,
};

function ImageTitle({ image, status }) {
	const lang = getMarketLanguage(image.market);
	const tooltip =
		Boolean(image.title) && image.title.length > 15
			? image.title
			: undefined;

	return (
		<h2>
			{status === "loading" && <Skeleton>Loading...</Skeleton>}
			{status === "loaded" && (
				<b title={tooltip} lang={lang}>
					{image.title}
				</b>
			)}
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

function WallpaperDetails({ image, onClose }) {
	return (
		<ModalDialog isOpen={Boolean(image)} onClose={onClose}>
			<ImageDetails image={image} />
		</ModalDialog>
	);
}

WallpaperDetails.propTypes = {
	image: cachedImageProps,
	onClose: PropTypes.func.isRequired,
};

function ModalDialog({ isOpen, onClose, children }) {
	const dialogRef = React.useRef();

	React.useEffect(() => {
		if (isOpen) {
			dialogRef.current?.showModal();
		} else {
			dialogRef.current?.close();
		}
	}, [isOpen]);

	return (
		<dialog
			ref={dialogRef}
			onCancel={onClose}
			onClose={onClose}
			aria-live="polite"
		>
			<form method="dialog">
				{children}
				<footer>
					<button type="submit">Back to image list</button>
				</footer>
			</form>
		</dialog>
	);
}

ModalDialog.propTypes = {
	isOpen: PropTypes.bool.isRequired,
	onClose: PropTypes.func.isRequired,
	children: PropTypes.node.isRequired,
};

function ImageDetails({ image }) {
	const lang = getMarketLanguage(image?.market);

	return (
		<>
			<h2 lang={lang}>{image?.title}</h2>
			<section>
				<p lang={lang} className="details">
					<a href={image?.fullResolution} rel="noreferrer noopener">
						<img
							src={image?.lowResolution}
							alt={image?.title}
							width="320"
							height="180"
						/>
					</a>
				</p>
				<p lang={lang}>{image?.copyright}</p>
				<p>
					<a href={image?.fullResolution} rel="noreferrer noopener">
						High resolution
					</a>
				</p>
				<p>
					<a
						href={image?.ultraHighResolution}
						rel="noreferrer noopener"
					>
						Ultra-high resolution
					</a>
				</p>
			</section>
		</>
	);
}

ImageDetails.propTypes = {
	image: cachedImageProps,
};

function getMarketLanguage(market) {
	let lang = undefined;
	if (Boolean(market) && !market.startsWith("en")) {
		lang = market;
	}
	return lang;
}
